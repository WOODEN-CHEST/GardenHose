using GardenHose.Engine.Frame;
using GardenHose.Engine.Frame.UI.Item;

using Microsoft.Xna.Framework;
using GardenHose.Engine.IO;
using GardenHose.Engine;
using GardenHose.Engine.Frame.UI.Animation;
using GardenHose.Engine.Frame.UI;
using System;

namespace GardenHose.Frames;

public class FrameMain : GameFrame
{
    // Fields.

    /* Animations */
    public FrameAnimation NumberTest;
    public FrameAnimation Background;


    /* Sounds */


    /* Layers */

    /* Layer0 */
    public Layer Layer0;
    public SpriteItem Backdrop;


    /* Layer1 */
    public Layer Layer1;
    public SpriteItem Test;
    public TextBox Text;


    // Private fields.


    // Constructors.
    public FrameMain() : base("Main")
    {
        CreateAnimations();
        CreateSounds();
        CreateLayers();
        MouseEventListener.AddClickListener(OnClick, true, MouseClickCondition.OnClick, MouseButton.Left);
    }



    // Methods.



    // Private methods.
    /* Initializers */
    private void CreateAnimations()
    {
        NumberTest = CreateAnimation(() => new Animation(2.5f, "1", "2", "3"));
        Background = CreateAnimation(() => new Animation(0f, "screen"));
    }

    private void CreateSounds()
    {

    }

    private void CreateLayers()
    {
        CreateLayer0();
        CreateLayer1();
    }

    private void CreateLayer0()
    {
        Layer0 = new(this);

        Backdrop = new(new Vector2(), new Vector2(1f, 1f), 0f, new AnimationInstance(Background.Get()));
        Backdrop.ActiveAnimation.Anim.Frames[0].Origin = new Vector2(0f, 0f);

        Backdrop.Tint = new Color(255, 255, 255);

        Layer0.AddDrawable(Backdrop);
    }

    /* Input handlers. */
    public void OnClick(MouseEventInfo info)
    {
        Test.StartPositionInterpolation(MouseEventListener.VirtualPositionCur, 1f);
        Test.StartRotationInterpolation(MathF.Atan2(
            MouseEventListener.VirtualPositionCur.Y - Test.Position.Y,
            MouseEventListener.VirtualPositionCur.X - Test.Position.X) + MathHelper.PiOver2, 1f);
    }


    private void CreateLayer1()
    {
        Layer1 = new(this);

        Test = new(new Vector2(1920f / 2f, 1080f / 2f), new Vector2(1f, 1f), 0f, new AnimationInstance(NumberTest.Get()));
        Test.PositionIntMethod = InterpolationMethod.EaseOut;
        Test.RotationIntMethod = InterpolationMethod.EaseOut;
        Test.Opacity = 255;
        Test.Brightness = 255;
        Layer1.AddDrawable(Test);

        Text = new(new Vector2(200, 200), new Vector2(1f, 1f), 0f, DynamicFont.GetFont("default"),
            "abcd", Color.Brown, float.MaxValue);
        Text.IsTypeable = true;
        Text.AllowNewlineTyping = false;
        Text.MaxCharacters = 30;

        Layer1.AddDrawable(Text);
    }

    // Inehrited methods.
    protected override void OnStart()
    {
        base.OnStart();
    }

    public override void Update()
    {

    }

    public override void Draw()
    {
        base.Draw();
    }
}
