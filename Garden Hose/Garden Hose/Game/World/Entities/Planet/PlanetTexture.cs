using GardenHose.Game.GameAssetManager;
using GardenHose.Game.World.Entities.Physical;
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
    internal Color Color { get; set; } = Color.White;

    internal float Opacity { get; set; } = 1f;

    internal PlanetTextureType Type { get; init; }

    float Scale { get; set; } = 1f;


    // Private fields.
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

        _textureEntries.Add(PlanetTextureType.DefaultAtmosphere, "planet_atmosphere_default");
    }


    // Constructors.
    internal PlanetTexture(PlanetTextureType type, Color color, float opacity, float scale = 1f)
    {
        Type = type;
        Color = color;
        Opacity = opacity;
        Scale = scale;
    }


    // Operators.
    public static implicit operator PhysicalEntityPartSprite(PlanetTexture texture)
    {
        return new PhysicalEntityPartSprite(_textureEntries[texture.Type])
        {
            ColorMask = texture.Color,
            Opacity = texture.Opacity,
            Scale = new Vector2(texture.Scale)
        };
    }
}