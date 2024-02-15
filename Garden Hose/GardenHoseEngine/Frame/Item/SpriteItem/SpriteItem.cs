using GardenHoseEngine.Frame.Animation;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics.CodeAnalysis;

namespace GardenHoseEngine.Frame.Item;

public class SpriteItem : ColoredItem, ICloneable
{
    // Fields.
    [MemberNotNull(nameof(_activeAnimation))]
    public virtual AnimationInstance ActiveAnimation
    {
        get => _activeAnimation;
        set => _activeAnimation = value ?? throw new ArgumentNullException(nameof(value));
    }
    public virtual Vector2 Size
    {
        get => _size;
        set
        {
            _size = value;
            TextureScale = _size / TextureSize;
        }
    }
    public virtual Vector2 TextureSize => new(ActiveAnimation.GetFrame().Texture.Width, ActiveAnimation.GetFrame().Texture.Height);
    internal virtual Vector2 TextureScale { get; private set; }
    public virtual Vector2 SpriteCenter => Position + (TextureSize * 0.5f - ActiveAnimation.GetFrame().Origin) * TextureScale;
    public SpriteEffects Effects { get; set; } = SpriteEffects.None;


    // Private fields.
    private AnimationInstance _activeAnimation;
    private Vector2 _size = new Vector2(50f, 50f);


    // Constructors.
    public SpriteItem(AnimationInstance animationInstance)
    {
        ActiveAnimation = animationInstance;
        Size = TextureSize;
    }

    public SpriteItem(AnimationInstance animationInstance, Vector2 size)
    {
        ActiveAnimation = animationInstance;
        Size = size;
    }

    public SpriteItem(AnimationInstance animationInstance, float scale)
    {
        ActiveAnimation = animationInstance;
        Size = TextureSize * scale;
    }


    // Inherited methods.
    public override void Draw(IDrawInfo info)
    {
        if (!IsDrawingNeeded) return;

        ActiveAnimation.Update(info.Time);

        info.SpriteBatch.Draw(
            ActiveAnimation.GetFrame().Texture,
            Display.ToRealPosition(Position),
            ActiveAnimation.TextureRegion,
            CombinedMask,
            Rotation,
            ActiveAnimation.GetFrame().Origin,
            Display.ToRealScale(TextureScale),
            Effects,
            IDrawableItem.DEFAULT_LAYER_DEPTH);
    }

    public virtual object Clone()
    {
        return new SpriteItem((AnimationInstance)_activeAnimation.Clone())
        {
            Size = Size,
            Position = Position,
            Rotation = Rotation,
            Effects = Effects,
            TextureScale = TextureScale,

            IsVisible = IsVisible,
            IsDrawingNeeded = IsDrawingNeeded,
            Brightness = Brightness,
            Opacity = Opacity,
            Mask = Mask
        };
    }
}
