using GardenHoseEngine.Frame.Animation;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics.CodeAnalysis;

namespace GardenHoseEngine.Frame.Item;

public class SpriteItem : ColoredItem
{
    // Fields.
    [MemberNotNull(nameof(_activeAnimation))]
    public virtual AnimationInstance ActiveAnimation
    {
        get => _activeAnimation;
        set => _activeAnimation = value ?? throw new ArgumentNullException(nameof(value));
    }

    public virtual Vector2 TextureSize => new(ActiveAnimation.GetFrame().Texture.Width, ActiveAnimation.GetFrame().Texture.Height);


    // Private fields.
    private AnimationInstance _activeAnimation;


    // Constructors.
    public SpriteItem(IVirtualConverter converter, AnimationInstance animationInstance) : base(converter)
    {
        ActiveAnimation = animationInstance;
    }

    public SpriteItem(IVirtualConverter converter, SpriteAnimation animation) : base(converter)
    {
        ActiveAnimation = animation.CreateInstance();
    }


    // Inherited methods.
    public override void Draw(float passedTimeSeconds, SpriteBatch spriteBatch)
    {
        if (!_ShouldDraw) return;

        spriteBatch.Draw(
            ActiveAnimation.GetFrame().Texture,
            Converter.ToRealPosition(Position),
            ActiveAnimation.TextureRegion,
            CombinedMask,
            Rotation,
            ActiveAnimation.GetFrame().Origin,
            Converter.ToRealScale(Scale),
            SpriteEffects.None,
            IDrawableItem.DEFAULT_LAYER_DEPTH);
    }
}
