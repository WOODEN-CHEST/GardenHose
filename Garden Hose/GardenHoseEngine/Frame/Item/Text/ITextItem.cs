using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace GardenHoseEngine.Frame.Item.Text;

public interface ITextItem : IColorMaskable
{
    public string Text { get; set; }
    public SpriteFont Font { get; set; }
    public float Scale { get; set; }
    public Origin TextOrigin { get; set; }
    public Vector2 PixelSize { get; }
    public SpriteEffects Effects { get; set; }
}