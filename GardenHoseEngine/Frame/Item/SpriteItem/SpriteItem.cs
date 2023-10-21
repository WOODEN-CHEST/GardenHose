﻿using GardenHoseEngine.Frame.Animation;
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
    public SpriteItem(AnimationInstance animationInstance) : base()
    {
        ActiveAnimation = animationInstance;
    }

    public SpriteItem(SpriteAnimation animation) : base()
    {
        ActiveAnimation = animation.CreateInstance();
    }


    // Inherited methods.
    public override void Draw()
    {
        if (!_ShouldDraw) return;

        GameFrameManager.SpriteBatch.Draw(
            ActiveAnimation.GetFrame().Texture,
            Display.ToRealPosition(Position),
            ActiveAnimation.TextureRegion,
            CombinedMask,
            Rotation,
            ActiveAnimation.GetFrame().Origin,
            Display.ToRealScale(Scale),
            SpriteEffects.None,
            IDrawableItem.DEFAULT_LAYER_DEPTH);
    }
}
