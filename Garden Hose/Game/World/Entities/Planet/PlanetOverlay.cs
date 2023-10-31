using GardenHose.Game.AssetManager;
using GardenHoseEngine;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities;


internal class PlanetOverlay
{
    // Internal fields.
    internal Color Color { get; set; }

    internal float Opacity { get; set; }

    internal PlanetOverlayType Type { get; init; }

    internal SpriteItem Item { get; private set; }

    internal Vector2 TextureScaling { get; private set; }


    // Constructors.
    internal PlanetOverlay(PlanetOverlayType type, Color color)
        : this(type, color, 1f) { }
       
    internal PlanetOverlay(PlanetOverlayType type, Color color, float opacity)
    {
        Type = type;
        Color = color;
        Opacity = opacity;
    }


    // Internal methods.
    internal void Load(GHGameAssetManager assetManager, float planetDiameter)
    {
        Item = new(Type switch
        {
            PlanetOverlayType.Gas1Overlay1 => assetManager.PlanetGas1Overlay1,
            PlanetOverlayType.Gas1Overlay2 => assetManager.PlanetGas1Overlay2,
            _ => throw new EnumValueException(nameof(Type), nameof(PlanetOverlayType),
                Type.ToString(), (int)Type)
        });
        
        TextureScaling = new(planetDiameter / Item.TextureSize.X, planetDiameter / Item.TextureSize.Y);
    }

    internal void Draw(Vector2 position, GameWorld world)
    {
        Item.Position.Vector = world.ToViewportPosition(position);
        Item.Scale.Vector = TextureScaling * world.Zoom;
        Item.Mask = Color;
        Item.Opacity = Opacity;
        Item.Draw();
    }
}