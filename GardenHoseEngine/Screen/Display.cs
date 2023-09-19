using GardenHoseEngine.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;

namespace GardenHoseEngine.Screen;

public class Display : IVirtualConverter
{
    // Fields.
    public readonly Vector2 MinimumWindowSize = new(144f, 80f);

    public bool IsFullScreen
    {
        get => _graphicsDeviceManager.IsFullScreen;
        set
        {
            if (value != IsFullScreen)
            {
                SetDisplay(value ? FullScreenSize : WindowedSize, value);
            }
        }
    }

    public Vector2 FullScreenSize
    {
        get => _fullScreenSize;
        set
        {
            if (IsFullScreen)
            {
                SetDisplay(value, true);
            }
            else
            {
                _fullScreenSize = ValidateScreenSize(value);
            }
        }
    }

    public Vector2 WindowedSize
    {
        get => _windowedSize;
        set
        {
            if (!IsFullScreen)
            {
                SetDisplay(value, false);
            }
            else
            {
                _windowedSize = ValidateScreenSize(value);
            }
        }
    }

    public Vector2 WindowSize
    {
        get => new(_window.ClientBounds.Width, _window.ClientBounds.Height);
        set => SetDisplay(value, IsFullScreen);
    }

    public Vector2 VirtualSize { get; init; }

    

    public Vector2 ViewportSize { get; private set; }

    public Vector2 ScreenSize { get; init; }

    public Vector2 ItemOffset { get; private set; }

    public float ItemScale { get; private set; }

    public float InverseItemScale { get; private set; }


    public event EventHandler? DisplayChanged;


    // Private fields.
    private readonly GraphicsDeviceManager _graphicsDeviceManager;
    private readonly GameWindow _window;
    private Vector2 _windowedSize;
    private Vector2 _fullScreenSize;


    // Constructors.
    internal Display(GraphicsDeviceManager graphicsManager, 
        GameWindow gameWindow, 
        Vector2 virtualSize,
        Vector2? windowSize,
        bool isFullScreen)
    {
        // Assign fields from args.
        _graphicsDeviceManager = graphicsManager ?? throw new ArgumentNullException(nameof(graphicsManager));
        _window = gameWindow ?? throw new ArgumentNullException(nameof(gameWindow));

        VirtualSize = ValidateScreenSize(virtualSize);
        gameWindow.ClientSizeChanged += OnWindowSizeChangeByUserEvent;

        // Initialize fields to default values.
        ScreenSize = new(_graphicsDeviceManager.GraphicsDevice.Adapter.CurrentDisplayMode.Width,
            _graphicsDeviceManager.GraphicsDevice.Adapter.CurrentDisplayMode.Height);
        _fullScreenSize = ScreenSize;

        SetDisplay(windowSize ?? ScreenSize / 1.5f, isFullScreen);
    }


    // Methods.
    public void CorrectWindowedSize()
    {
        if (!IsFullScreen)
        {
            SetDisplay(ViewportSize, false);
        }
    }


    // Private static methods.
    private void SetDisplay(Vector2 newSize, bool isFullScreen)
    {
        newSize = ValidateScreenSize(newSize);

        _graphicsDeviceManager.IsFullScreen = isFullScreen;
        _graphicsDeviceManager.PreferredBackBufferWidth = (int)newSize.X;
        _graphicsDeviceManager.PreferredBackBufferHeight = (int)newSize.Y;
        _graphicsDeviceManager.ApplyChanges();

        UpdateDisplayInfo();
    }

    private void UpdateDisplayInfo()
    {
        // Update size info.
        if (IsFullScreen)
        {
            _fullScreenSize = WindowSize;
        }
        else
        {
            _windowedSize = WindowSize;
        }

        // Calculate properties.
        ItemScale = Math.Min(WindowSize.X / VirtualSize.X, WindowSize.Y / VirtualSize.Y);
        InverseItemScale = 1 / ItemScale;
        ViewportSize = VirtualSize * ItemScale;
        ItemOffset = new((WindowSize.X - ViewportSize.X) / 2f, (WindowSize.Y - ViewportSize.Y) / 2f);

        DisplayChanged?.Invoke(this, EventArgs.Empty);
    }

    private Vector2 ValidateScreenSize(Vector2 size)
    {
        if (!(float.IsFinite(size.X) && (size.X > 0)
            && float.IsFinite(size.Y) && (size.Y > 0)))
        {
            throw new ArgumentException($"Invalid virtual display size: {size}", nameof(size));
        }

        size.X = Math.Max(size.X, MinimumWindowSize.X);
        size.Y = Math.Max(size.Y, MinimumWindowSize.Y);
        return size;
    }

    private void OnWindowSizeChangeByUserEvent(object? sender, EventArgs args)
    {
        if ((_window.ClientBounds.Width < MinimumWindowSize.X) 
            || (_window.ClientBounds.Height < MinimumWindowSize.Y))
        {
            SetDisplay(MinimumWindowSize, IsFullScreen);
        }
        else
        {
            UpdateDisplayInfo();
        }
    }


    // Inherited methods.
    public Vector2 ToRealPosition(Vector2 virtualVector) => (virtualVector * ItemScale) + ItemOffset;

    public Vector2 ToVirtualPosition(Vector2 realVector) => (realVector - ItemOffset) * InverseItemScale;

    public Vector2 ToRealScale(Vector2 virtualScale) => virtualScale * ItemScale;

    public Vector2 ToVirtualScale(Vector2 virtualScale) => virtualScale * InverseItemScale;
}