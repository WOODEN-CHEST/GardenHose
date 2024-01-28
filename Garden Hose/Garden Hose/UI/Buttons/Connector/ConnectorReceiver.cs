using GardenHoseEngine.Frame.Animation;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework;

namespace GardenHose.UI.Buttons.Connector;

internal class ConnectorReceiver
{
    // Internal static fields.
    internal static readonly Vector2 RECEIVER_SIZE = new(167f, 255f);


    // Internal fields.
    internal Vector2 Position
    {
        get => _receiver.Position;
        set
        {
            _receiver.Position = value;
            _lights.Position = value;
        }
    }

    internal Color LightColor
    {
        get => _lights.Mask;
        set
        {
            _lights.Mask = value;
        }
    }

    internal float Opacity
    {
        get => _receiver.Opacity;
        set
        {
            _lights.Opacity = value;
            _receiver.Opacity = value;
        }
    }

    internal float Scale
    {
        get => _scale;
        set
        {
            _scale = value;
            _receiver.Size = RECEIVER_SIZE * value;
            _lights.Size = RECEIVER_SIZE * value;
        }
    }

    // Private fields.
    private readonly SpriteItem _receiver;
    private readonly SpriteItem _lights;
    private float _scale;


    // Constructors.
    internal ConnectorReceiver(Direction direction, ConnectorAssetCollection assets)
    {
        float Rotation = ConnectorElement.GetConnectorAngle(direction);

        _receiver = new(assets.Receiver, RECEIVER_SIZE);
        _lights = new(assets.ReceiverLights, RECEIVER_SIZE);

        _receiver.Rotation = Rotation;
        _lights.Rotation = Rotation;
    }


    // Internal methods.
    internal void Draw(IDrawInfo info)
    {
        _lights.Draw(info);
        _receiver.Draw(info);
    }
}