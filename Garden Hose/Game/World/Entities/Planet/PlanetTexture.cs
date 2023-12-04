using GardenHose.Game.AssetManager;
using GardenHoseEngine;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities.Planet;


internal class PlanetTexture
{
    // Internal fields.
    internal Color Color { get; set; }

    internal float Opacity { get; set; }

    internal PlanetTextureType Type { get; init; }

    internal SpriteItem Item { get; private set; }

    internal Vector2 TextureScaling { get; private set; }


    // Private static fields.
    private readonly static Dictionary<PlanetTextureType, string> _textureEntries = new();


    // Static constructors.
    static PlanetTexture()
    {
        _textureEntries.Add(PlanetTextureType.Gas1Surface, "planet_layer_gassurface1");
        _textureEntries.Add(PlanetTextureType.Gas1Overlay1, "planet_layer_gasoverlay1");
        _textureEntries.Add(PlanetTextureType.Gas1Overlay2, "planet_layer_gasoverlay2");

        _textureEntries.Add(PlanetTextureType.RockSurface1, "planet_layer_rocksurface1");
        _textureEntries.Add(PlanetTextureType.WaterSurface1, "planet_layer_watersurface1");

        _textureEntries.Add(PlanetTextureType.Clouds1, "planet_layer_clouds1");
        _textureEntries.Add(PlanetTextureType.Clouds2, "planet_layer_clouds2");
        _textureEntries.Add(PlanetTextureType.Clouds3, "planet_layer_clouds3");
        _textureEntries.Add(PlanetTextureType.Clouds4, "planet_layer_clouds4");
        _textureEntries.Add(PlanetTextureType.Clouds5, "planet_layer_clouds5");
    }


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
        Item = new(assetManager.GetAnimation(_textureEntries[Type])!);
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