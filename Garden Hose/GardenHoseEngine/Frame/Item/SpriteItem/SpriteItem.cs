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

    public virtual Vector2? TargetTextureSize { get; set; } = null;


    // Private fields.
    private AnimationInstance _activeAnimation;


    // Constructors.
    public SpriteItem(AnimationInstance animationInstance) : base()
    {
        ActiveAnimation = animationInstance;
    }

    public SpriteItem(SpriteAnimation animation) : base()
    {
        ActiveAnimation = animation.CreateInstance();
    }

    public SpriteEffects Effects { get; set; } = SpriteEffects.None;


    // Inherited methods.
    public override void Draw()
    {
        if (!_ShouldDraw) return;

        Vector2 SpriteSize = TargetTextureSize == null ? Scale : (TargetTextureSize.Value / TextureSize * Scale);

        GameFrameManager.s_spriteBatch.Draw(
            ActiveAnimation.GetFrame().Texture,
            Display.ToRealPosition(Position),
            ActiveAnimation.TextureRegion,
            CombinedMask,
            Rotation,
            ActiveAnimation.GetFrame().Origin,
            Display.ToRealScale(SpriteSize),
            Effects,
            IDrawableItem.DEFAULT_LAYER_DEPTH);
    }
}
