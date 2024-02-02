using GardenHoseEngine.IO;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using System;
using System.Runtime.CompilerServices;

namespace GardenHose.Game.World.Player;

internal class PlayerCamera : IWorldCamera
{
    // Fields.
    public Vector2 CameraCenter
    {
        get => _cameraCenter;
        set
        {
            _cameraCenter = value;
            ObjectVisualOffset = (Display.VirtualSize / 2f) - (_cameraCenter * Zoom);
        }
    }

    public Vector2 ObjectVisualOffset { get; private set; }

    public float Zoom
    {
        get => _zoom;
        set
        {
            _zoom = value;
            InverseZoom = 1f / value;
            CameraCenter = _cameraCenter; // Forces object visual position update.
        }
    }

    public float InverseZoom { get; private set; } = 1f;


    // Internal fields.
    internal const float CAMERA_MOVEMENT_SPEED = 5f;
    internal const float MOUSE_MOVEMENT_OFFSET_FACTOR = 0.1f;
    internal const float MOTION_OFFSET_FACTOR = 0.5f;
    internal const float ROTATION_OFFSET_AMOUNT = 75f;
    internal const float ZOOM_SPEED = 3f;
    internal const float ZOOM_AMOUNT = 1.2f;
    internal const float MAX_ZOOM = 10f;
    internal const float MIN_ZOOM = 0.75f;
    internal const float MOUSE_ZOOM_FACTOR = 0.005f;

    internal WorldPlayer GamePlayer { get; private init; }
    internal float TargetZoom { get; private set; } = 1f;
    internal Vector2 TargetPosition { get; private set; }


    // Private fields.
    private Vector2 _cameraCenter = Vector2.Zero;
    private float _zoom = 1f;

    private IInputListener _zoomInListener;
    private IInputListener _zoomOutListener;


    // Constructors.
    internal PlayerCamera(WorldPlayer player)
    {
        GamePlayer = player ?? throw new ArgumentNullException(nameof(player));

        _zoomInListener = MouseListenerCreator.Scroll(true, ScrollDirection.Up, OnPlayerZoomInEvent);
        _zoomOutListener = MouseListenerCreator.Scroll(true, ScrollDirection.Down, OnPlayerZoomOutEvent);
        UserInput.AddListener(_zoomInListener);
        UserInput.AddListener(_zoomOutListener);
    }

    // Internal methods.
    internal void Tick(GHGameTime time)
    {
        Vector2 MouseOffsetFromCenter = (UserInput.VirtualMousePosition.Current - (Display.VirtualSize * 0.5f));

        // Position.
        Vector2 MouseOffset = MouseOffsetFromCenter * MOUSE_MOVEMENT_OFFSET_FACTOR * InverseZoom;
        Vector2 MotionOffset = GamePlayer.SpaceShip.Motion * MOTION_OFFSET_FACTOR * InverseZoom;
        Vector2 FacingOffset = Vector2.Transform(-Vector2.UnitY, Matrix.CreateRotationZ(GamePlayer.SpaceShip.Rotation))
            * ROTATION_OFFSET_AMOUNT * InverseZoom;

        TargetPosition = GamePlayer.SpaceShip.Position + MouseOffset + FacingOffset;
        CameraCenter += (TargetPosition - CameraCenter) * 
            Math.Min(time.WorldTime.PassedTimeSeconds * CAMERA_MOVEMENT_SPEED, 1f);

        // Zoom.
        float MouseOffsetZoom = 1f - (MouseOffsetFromCenter.Length() / (Display.VirtualSize.Length() * 0.5f) * MOUSE_ZOOM_FACTOR);
        Zoom = (Zoom + Math.Min((TargetZoom - Zoom) * time.WorldTime.PassedTimeSeconds * ZOOM_SPEED, 1f)) * MouseOffsetZoom; 
    }


    // Private methods.
    private void OnPlayerZoomInEvent(object? sender, EventArgs args)
    {
        TargetZoom = Math.Min(TargetZoom * ZOOM_AMOUNT, MAX_ZOOM);
    }

    private void OnPlayerZoomOutEvent(object? sender, EventArgs args)
    {
        TargetZoom = Math.Max(TargetZoom / ZOOM_AMOUNT, MIN_ZOOM);
    }


    // Inherited methods.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 ToViewportPosition(Vector2 worldPosition)
    {
        return (worldPosition * Zoom) + ObjectVisualOffset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 ToWorldPosition(Vector2 viewportPosition)
    {
        return (viewportPosition - ObjectVisualOffset) / Zoom;
    }
}