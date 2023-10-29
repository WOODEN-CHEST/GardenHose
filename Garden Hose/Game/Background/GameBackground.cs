using GardenHose.Game.Background;
using GardenHoseEngine;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game;

internal class GameBackground : IDrawableItem, ITimeUpdatable
{
    // Fields.
    public bool IsVisible { get; set; }

    public Effect? Shader { get; set; }


    // Internal fields.
    internal float SmallStarCount { get; init; } = 0f;

    internal float MediumStarCount { get; init; } = 0f;

    internal float LargeStarCount { get; init; } = 0f;

    internal BackgroundImage Image { get; init; }
    

    // Private fields.
    private



    // Constructors.
    public GameBackground(BackgroundImage image)
    {

    }



    // Methods.
    internal void CreateBackground()
    {

    }


    // Private methods.
    private Vector2 GetRandomLocation()
    {
        return new Vector2(Random.Shared.Next((int)Display.VirtualSize.X), 
            Random.Shared.Next((int)Display.VirtualSize.Y));
    }


    // Inherited methods.
    public void Update()
    {
        
    }

    public void Draw()
    {
        
    }
}