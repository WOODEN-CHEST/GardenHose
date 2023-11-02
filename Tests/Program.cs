using GardenHose.Game.World.Entities;
using Microsoft.Xna.Framework;

namespace Tests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GardenHose.Game.World.Entities.Ray ARay = new(-250f, 57f, 7f);
            Circle ACircle = new Circle(9, new Vector2(-3f, -2f));
            Circle BCircle = new Circle(6, new Vector2(3f, -10f));

            Vector2[] points = Circle.GetIntersections(ACircle, BCircle);
        }
    }
}