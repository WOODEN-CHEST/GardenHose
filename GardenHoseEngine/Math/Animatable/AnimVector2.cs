using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GardenHoseEngine;

public struct AnimVector2
{
    // Fields.
    public Vector2 Vector;
    public bool IsLooped { get; set; }= false;
    public float Speed
    {
        get => _speed;
        set
        {
            if (float.IsNaN(_speed) || float.IsInfinity(_speed))
            {
                throw new ArgumentException($"Invalid animation speed: {value}");
            }

            _speed = value;
        }
    }


    // Private fields.
    private float _speed = 1f;


    // Constructors.
    public AnimVector2(Vector2 vector)
    {
        Vector = vector;
    }
    
    public AnimVector2(float value)
    {
        Vector = new(value);
    }
    
    public AnimVector2()
    {
        Vector = Vector2.Zero;
    }


    // Methods.
    public void Start()
    {

    }

    public void Stop()
    {

    }

    public void SetTime()
    {

    }


    // Private methods.



    // Operators.
    public static implicit operator Vector2(AnimVector2 animVector2) => animVector2.Vector;

    public static implicit operator AnimVector2(Vector2 vector) => new(vector);
}