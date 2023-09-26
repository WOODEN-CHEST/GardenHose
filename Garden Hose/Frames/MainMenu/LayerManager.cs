using GardenHoseEngine.Frame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Frames.MainMenu;

internal class MainFrameLayerManager : FrameComponentManager<MainMenuFrame>
{
    // Internal fields.
    internal ILayer BackgroundLayer { get; private init; } = new Layer("background");

    internal ILayer UILayer { get; private init; } = new Layer("ui");

    internal ILayer OverlayLayer { get; private init; } = new Layer("overlay");


    internal double FadeStep
    {
        get => _fadeStep;
        set
        {
            _fadeStep = value;
            _isFading = true;
        }
    }


    // Private fields.
    private double _fadeStep = 0.8d;
    private float _fadeBrightness = 0f;
    private bool _isFading = true;


    // Constructors.
    public MainFrameLayerManager(MainMenuFrame parentFrame) : base(parentFrame)
    {
        parentFrame.AddLayer(BackgroundLayer);
        parentFrame.AddLayer(UILayer);
        parentFrame.AddLayer(OverlayLayer);
        OverlayLayer.Opacity = 0f;
    }


    // Private methods.
    private void FadeLayerBrightness(float passedTimeSeconds)
    {
        _fadeBrightness = Math.Clamp(_fadeBrightness + passedTimeSeconds, 0f, 1f);
        if (_fadeBrightness is 0f or 1f)
        {
            _isFading = false;
        }

        BackgroundLayer.Brightness = _fadeBrightness;
        UILayer.Brightness = _fadeBrightness;
        OverlayLayer.Brightness = _fadeBrightness;
    }


    // Inherited methods.
    internal override void Update(float passedTimeSeconds)
    {
        if (_isFading) FadeLayerBrightness(passedTimeSeconds);
    }
}
