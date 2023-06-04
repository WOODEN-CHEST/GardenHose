using Microsoft.Xna.Framework;
using System;

namespace GardenHose.Engine.Frame.UI.Item;

public abstract class DrawableItem : IDrawableItem
{
    // Fields.
    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            _isVisible = value;
            UpdateShouldRender();
        }
    }

    public float Opacity
    {
        get => _opacity;
        set
        {
            _opacity = Math.Clamp(value, 0f, 1f);
            RealColorMask.A = (byte)(255f * _opacity);
            UpdateShouldRender();
        }
    }

    public float Brightness
    {
        get => _brightness;
        set
        {
            _brightness = Math.Clamp(value, 0f, 1f);
            UpdateColorMaskRGB();
        }
    }

    public Color Tint
    {
        get => _tint;
        set
        {
            _tint = value;
            _tint.A = RealColorMask.A = (byte)(255f * _opacity);
            UpdateColorMaskRGB();
        }
    }

    public Vector2 Position
    {
        get => VirtualPosition;
        set
        {
            VirtualPosition = value;

            RealPosition = value;

            if (IsPositionScaledX) RealPosition.X = (RealPosition.X) * DisplayInfo.XScale;
            if (IsPositionScaledY) RealPosition.Y = (RealPosition.Y) * DisplayInfo.YScale;

            if (IsPositionOffsetX) RealPosition.X += DisplayInfo.XOffset;
            if (IsPositionOffsetY) RealPosition.Y += DisplayInfo.YOffset;
        }
    }

    public float Rotation { get; set; }

    public Vector2 Scale
    {
        get => VirtualScale;
        set
        {
            VirtualScale = value;

            RealScale = value;

            if (IsSizeRelative) RealScale *= DisplayInfo.ItemScale;
        }
    }

    public bool IsInterpolating
    {
        get => _isPositionInterpolating || _isScaleInterpolating || _isRotationInterpolating;
    }

    public InterpolationMethod PositionIntMethod = InterpolationMethod.Linear;
    public InterpolationMethod ScaleIntMethod = InterpolationMethod.Linear;
    public InterpolationMethod RotationIntMethod = InterpolationMethod.Linear;


    /* Options for changing how the position and scale is calculated. */
    /* For world objects, they should remain the default value.
     * For UI elements, the Offset may be changed to accommodate for
     * different screen sizes. IsSizeRelative should almost never be false. */
    public DisplaySide RelativeOffsetSide = DisplaySide.Top;

    public bool IsPositionOffsetX = true;
    public bool IsPositionOffsetY = true;

    public bool IsPositionScaledX = true;
    public bool IsPositionScaledY = true;

    public bool IsSizeRelative = true;


    // Protected fields.
    protected Vector2 VirtualPosition;
    protected Vector2 RealPosition;

    protected Vector2 VirtualScale;
    protected Vector2 RealScale;

    protected Color RealColorMask = new Color(255, 255, 255, 255);
    protected bool ShouldRender { get; private set; } = true;


    // Private fields.
    private Vector2 _posIntStart;
    private Vector2 _posIntEnd;
    private double _posIntTargetTime;
    private double _posIntElapsedTime;
    private bool _isPositionInterpolating;


    private Vector2 _scaIntStart;
    private Vector2 _scaIntEnd;
    private double _scaIntTargetTime;
    private double _scaIntElapsedTime;
    private bool _isScaleInterpolating;

    private float _rotIntStart;
    private float _rotIntEnd;
    private double _rotIntTargetTime;
    private double _rotIntElapsedTime;
    private bool _isRotationInterpolating;

    private bool _isVisible = true;
    private float _opacity = 1f;
    private float _brightness = 1f;
    private Color _tint = new Color(255, 255, 255, 255);


    // Constructors.
    public DrawableItem() { }


    // Methods.
    public void StartPositionInterpolation(Vector2 end, double timeSeconds)
    {
        if (timeSeconds <= 0 || !double.IsNormal(timeSeconds))
            throw new ArgumentOutOfRangeException(timeSeconds.ToString());

        _isPositionInterpolating = true;
        _posIntStart = VirtualPosition;
        _posIntEnd = end;

        _posIntTargetTime = timeSeconds;
        _posIntElapsedTime = 0d;
    }

    public void StopPositionInterpolation() => _isPositionInterpolating = false;

    public void StartScaleInterpolation(Vector2 end, double timeSeconds)
    {
        if (timeSeconds <= 0 || !double.IsNormal(timeSeconds))
            throw new ArgumentOutOfRangeException(timeSeconds.ToString());

        _isScaleInterpolating = true;
        _scaIntStart = VirtualScale;
        _scaIntEnd = end;
        _scaIntTargetTime = timeSeconds;
        _scaIntElapsedTime = 0d;
    }

    public void StopScaleInterpolation() => _isScaleInterpolating = false;

    public void StartRotationInterpolation(float end, double timeSeconds)
    {
        if (timeSeconds <= 0 || !double.IsNormal(timeSeconds))
            throw new ArgumentOutOfRangeException(timeSeconds.ToString());

        _isRotationInterpolating = true;
        _rotIntStart = Rotation;
        _rotIntEnd = end;
        _rotIntTargetTime = timeSeconds;
        _rotIntElapsedTime = 0d;
    }

    public void StopRotationInterpolation() => _isRotationInterpolating = false;

    public void StopInterpolation()
    {
        _isPositionInterpolating = false;
        _isScaleInterpolating = false;
        _isRotationInterpolating = false;
    }


    // Private methods.
    private void UpdateShouldRender() => ShouldRender = _isVisible && (RealColorMask.A != 0);

    private void UpdateColorMaskRGB()
    {
        RealColorMask.R = (byte)(_tint.R * _brightness);
        RealColorMask.G = (byte)(_tint.G * _brightness);
        RealColorMask.B = (byte)(_tint.B * _brightness);
    }


    // Inherited methods.
    public virtual void OnDisplayChange()
    {
        Position = VirtualPosition;
        Scale = VirtualScale;
    }

    public virtual void Draw()
    {
        double Progress;

        if (_isPositionInterpolating)
        {
            _posIntElapsedTime += GameFrame.Time.ElapsedGameTime.TotalSeconds;
            Progress = _posIntElapsedTime / _posIntTargetTime;

            Position = GHMath.Interpolate(_posIntStart, _posIntEnd, (float)Progress, PositionIntMethod);

            if (Progress > 1d) _isPositionInterpolating = false;
        }

        if (_isScaleInterpolating)
        {
            _scaIntElapsedTime += GameFrame.Time.ElapsedGameTime.TotalSeconds;
            Progress = _scaIntElapsedTime / _scaIntTargetTime;

            Position = GHMath.Interpolate(_scaIntStart, _scaIntEnd, (float)Progress, ScaleIntMethod);

            if (Progress > 1d) _isScaleInterpolating = false;
        }

        if (_isRotationInterpolating)
        {
            _rotIntElapsedTime += GameFrame.Time.ElapsedGameTime.TotalSeconds;
            Progress = _rotIntElapsedTime / _rotIntTargetTime;

            Rotation = GHMath.Interpolate(_rotIntStart, _rotIntEnd, (float)Progress, RotationIntMethod);

            if (Progress > 1d) _isRotationInterpolating = false;
        }
    }
}