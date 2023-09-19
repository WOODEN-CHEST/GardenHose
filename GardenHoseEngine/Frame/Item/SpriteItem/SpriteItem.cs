using GardenHoseEngine.Frame.Animation;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics.CodeAnalysis;

namespace GardenHoseEngine.Frame.Item;

public class SpriteItem : ColoredItem, ISpriteItem
{
    // Fields.
    [MemberNotNull(nameof(_activeAnimation))]
    public AnimationInstance ActiveAnimation
    {
        get => _activeAnimation;
        set => _activeAnimation = value ?? throw new ArgumentNullException(nameof(value));
    }


    // Private fields.
    private AnimationInstance _activeAnimation;


    // Constructors.
    public SpriteItem(ITimeUpdater updater, IVirtualConverter converter, IDrawer drawer, 
        AnimationInstance animationInstance) : base(updater, converter, drawer)
    {
        ActiveAnimation = animationInstance;
    }

    public SpriteItem(ITimeUpdater updater, IVirtualConverter converter, IDrawer drawer,
        SpriteAnimation animation) : base(updater, converter, drawer)
    {
        ActiveAnimation = animation.CreateInstance(updater);
    }


    // Inherited methods.
    public override void Draw(TimeSpan passedTime, SpriteBatch spriteBatch)
    {
        if (!ShouldDraw) return;

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
