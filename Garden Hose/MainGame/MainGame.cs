using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NAudio.Dsp;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;

namespace GardenHose;

public class MainGame : Game
{
    // Static fields.
    public static MainGame Instance { get; private set; }
    public static GraphicsDeviceManager GraphicsManager { get; private set; }

    MouseState mouseState;
    WaveOutEvent PlaybackDevice;
    MixingSampleProvider Mixer = new(WaveFormat.CreateCustomFormat(WaveFormatEncoding.IeeeFloat,
        44100, 2, 2 * 44100 * sizeof(float), 8, 32));
    Passer Filter;
    AudioFileReader Reader;


    // Constructors.
    public MainGame()
    {
        GraphicsManager = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        Instance = this;

        IsMouseVisible = true;
        GraphicsManager.PreferredBackBufferWidth = 1280;
        GraphicsManager.PreferredBackBufferHeight = 720;
    }


    // Inherited methods.
    protected override void LoadContent()
    {

    }

    [STAThread]
    protected override void Initialize()
    {
        base.Initialize();

        PlaybackDevice = new();

        //Reader = new AudioFileReader(@"C:\Users\User\Desktop\click2.mp3");
        //Filter = new(Reader.ToSampleProvider(), 0);


        Mixer.ReadFully = true;
        PlaybackDevice.DesiredLatency = 50;
        PlaybackDevice.Init(Mixer);
        PlaybackDevice.Play();
    }

    protected override void Update(GameTime gameTime)
    {
        if (Mouse.GetState().LeftButton == ButtonState.Pressed
             && mouseState.LeftButton == ButtonState.Released)
        {
            Mixer.AddMixerInput(new AudioFileReader(@"C:\Users\User\Desktop\click2.mp3").ToSampleProvider());
        }

        mouseState = Mouse.GetState();
    }

    protected override void Draw(GameTime gameTime)
    {

    }
}

internal class Passer : ISampleProvider
{
    private ISampleProvider Source;
    public BiQuadFilter Filter { get; private set; }
    public int CutoffFrequency
    {
        get => _cutoffFrequency;
        set => _cutoffFrequency = value;
    }
    private int _cutoffFrequency;

    public WaveFormat WaveFormat => Source.WaveFormat;



    public Passer(ISampleProvider source, int frequency)
    {
        CutoffFrequency = frequency;
        Filter = BiQuadFilter.HighPassFilter(44100, CutoffFrequency, 1f);
        Source = source;
    }

    public int Read(float[] buffer, int offset, int count)
    {
        int changed = Source.Read(buffer, offset, count);

        for (int i = offset; i < (offset + count); i++)
        {
            buffer[i] = Filter.Transform(buffer[i]);
        }

        return changed;
    }
}