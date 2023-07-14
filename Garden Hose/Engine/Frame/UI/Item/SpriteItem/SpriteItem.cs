using GardenHose.Engine.Frame.UI.Animation;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace GardenHose.Engine.Frame.UI.Item;

public class SpriteItem : ColoredItem, ISpriteItem
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
    public SpriteItem(AnimationInstance animation)
    {
        ActiveAnimation = animation;
    }


    // Inherited methods.
    public override void Draw()
    {
        base.Draw();
        if (!ShouldDraw) return;

        AnimationFrame Frame = _activeAnimation.GetFrame();

        GameFrame.s_drawBatch.Draw(Frame.Texture,
        RealPosition,
        _activeAnimation.TextureRegion,
        RealColorMask,
        Rotation,
        Frame.Origin,
        RealScale,
        SpriteEffects.None,
        1f);
    }
}
