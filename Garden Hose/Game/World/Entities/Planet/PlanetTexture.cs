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


internal class PlanetTexture
{
    // Internal fields.
    internal Color Color { get; set; }

    internal float Opacity { get; set; }

    internal PlanetTextureType Type { get; init; }

    internal SpriteItem Item { get; private set; }

    internal Vector2 TextureScaling { get; private set; }


    // Constructors.
    internal PlanetTexture(PlanetTextureType type, Color color)
        : this(type, color, 1f) { }
       
    internal PlanetTexture(PlanetTextureType type, Color color, float opacity)
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
            PlanetTextureType.Gas1Surface => assetManager.PlanetGas1Surface,
            PlanetTextureType.Gas1Overlay1 => assetManager.PlanetGas1Overlay1,
            PlanetTextureType.Gas1Overlay2 => assetManager.PlanetGas1Overlay2,

            PlanetTextureType.Rock1Land => assetManager.PlanetRock1Land,
            PlanetTextureType.Water => assetManager.PlanetWater,

            PlanetTextureType.Clouds1 => assetManager.PlanetClouds1,
            PlanetTextureType.Clouds2 => assetManager.PlanetClouds2,
            PlanetTextureType.Clouds3 => assetManager.PlanetClouds3,
            PlanetTextureType.Clouds4 => assetManager.PlanetClouds4,
            PlanetTextureType.Clouds5 => assetManager.PlanetClouds5,

            _ => throw new EnumValueException(nameof(Type), nameof(PlanetTextureType),
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