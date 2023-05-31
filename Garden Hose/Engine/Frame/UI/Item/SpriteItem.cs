using GardenHose.Engine.Frame.UI.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;


namespace GardenHose.Engine.Frame.UI.Item;

public class SpriteItem : DrawableItem
{
    // Fields.
    public AnimationInstance ActiveAnimation
    {
        get => _activeAnimation;
        set => _activeAnimation = value ?? throw new ArgumentNullException(nameof(value));
    }
    

    // Private fields.
    private AnimationInstance _activeAnimation = null;


    // Constructors.
    public SpriteItem(Vector2 position, Vector2 scale, float rotation, AnimationInstance animation) 
        : base(position, scale, rotation)
    {
        ActiveAnimation = animation;
        Position = position;
    }


    // Inherited methods.
    public override void Draw()
    {
        if (!IsVisible) return;

        base.Draw();

        AnimationFrame Frame = _activeAnimation.GetFrame();

        GameFrame.DrawBatch.Draw(
        Frame.Texture,
        RealPosition, // Top left position from which to draw.
        _activeAnimation.TextureRegion, // Optional region from texture.
        RealColorMask, // Color tint.
        Rotation,
        Frame.Origin, // Texture origin.
        RealScale,
        SpriteEffects.None, // Effects.
        1f); // Layer depth.
    }
}
