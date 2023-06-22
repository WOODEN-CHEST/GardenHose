using GardenHose.Engine.Frame.UI.Animation;
using GardenHose.Engine.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GardenHose.Engine.Frame;


public partial class GameFrame
{
    // Fields.
    public readonly List<Layer> Layers = new();
    public readonly HashSet<IUpdateableItem> UpdateableItems = new();
    public readonly HashSet<InputListener> InputListeners = new();

    public string Name
    {
        get => name;
        set
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value.Length == 0) throw new ArgumentException("GameFrame's name is empty");
            name = value;
        }
    }

    public bool IsLoaded { get; private set; } = false;


    // Protected fields.
    protected string name = "Default Frame Name";


    // Private fields.
    private readonly List<FrameAnimation> _animations = new();
    private readonly List<FrameSound> _sounds = new();


    // Constructors.
    public GameFrame(string name)
    {
        Name = name;
    }


    // Methods.

    /* Loops */
    public virtual void Update()
    {
        foreach (var Item in UpdateableItems) Item.Update();
    }

    public virtual void Draw()
    {
        if (Layers.Count == 0) return;

        Effect Shader = null;
        bool BatchStarted = false;

        foreach (Layer L in Layers)
        {
            // Set the shader if needed.
            if (Shader != L.Shader)
            {
                if (BatchStarted) s_drawBatch.End();
                BatchStarted = false;
            }
            if (!BatchStarted)
            {
                Shader = L.Shader;
                s_drawBatch.Begin(SpriteSortMode.Deferred,
                    BlendState.NonPremultiplied,
                    SamplerState.LinearClamp,
                    DepthStencilState.None,
                    RasterizerState.CullCounterClockwise,
                    Shader);
                BatchStarted = true;
            }

            // Draw.
            L.Draw();
        }

        s_drawBatch.End();
    }

    public virtual void Restart()
    {
        OnStart();
    }


    // Protected methods.
    /* Management */
    protected FrameAnimation CreateAnimation(Func<Animation> animationCreator)
    {
        FrameAnimation Anim = new(animationCreator);
        _animations.Add(Anim);
        return Anim;
    }

    protected FrameSound CreateSound(string relativePath)
    {
        FrameSound Sound = new(relativePath);
        _sounds.Add(Sound);
        return Sound;
    }


    /* Events */
    protected virtual void Load()
    {
        IsLoaded = true;
    }

    protected virtual void OnStart()
    {

    }

    protected virtual void Unload()
    {
        foreach (var Animation in _animations) Animation.Dispose();
        foreach (var Sound in _sounds) Sound.Dispose();
        foreach (var Item in UpdateableItems) Item.Delete();
        foreach (var Listener in InputListeners) Listener.StopListening();

        IsLoaded = false;
    }

    protected virtual void OnEnd()
    {

    }
}
