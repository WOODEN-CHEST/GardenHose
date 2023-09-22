using Microsoft.Xna.Framework;
using System;

namespace GardenHoseEngine;

public struct FloatColor
{
    // Fields.
    public const float MIN_VALUE = 0f;
    public const float MAX_VALUE = 1f;
    public const float STEP = 1f / byte.MaxValue;

    public Color Color
    {
        get => (Color)this;
        set
        {
            R = value.R / 255f;
            G = value.G / 255f;
            B = value.B / 255f;
            A = value.A / 255f;
        }
    }

    public float R 
    {
        get => _r; 
        set => _r = Math.Clamp(value, MIN_VALUE, MAX_VALUE); 
    }

    public float G
    {
        get => _g;
        set => _g = Math.Clamp(value, MIN_VALUE, MAX_VALUE);
    }

    public float B
    {
        get => _b;
        set => _b = Math.Clamp(value, MIN_VALUE, MAX_VALUE);
    }

    public float A
    {
        get => _a;
        set => _a = Math.Clamp(value, MIN_VALUE, MAX_VALUE);
    }


    // Private fields.
    private float _r;
    private float _g;
    private float _b;
    private float _a;


    // Constructors.
    public FloatColor(Color color)
    {
        R = (float)color.R / byte.MaxValue;   
        G = (float)color.R / byte.MaxValue;   
        B = (float)color.R / byte.MaxValue;   
        A = (float)color.R / byte.MaxValue;   
    }

    public FloatColor(float r, float g, float b, float a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }


    // Static methods.
    public static FloatColor InterpolateRGB(FloatColor color1, FloatColor color2, float amount)
    {
        color1.R = color1.R + (color2.R - color1.R) * amount;
        color1.G = color1.G + (color2.G - color1.G) * amount;
        color1.B = color1.B + (color2.B - color1.B) * amount;
        return color1;
    }

    public static FloatColor InterpolateRGBA(FloatColor color1, FloatColor color2, float amount)
    {
        color1.R = color1.R + (color2.R - color1.R) * amount;
        color1.G = color1.G + (color2.G - color1.G) * amount;
        color1.B = color1.B + (color2.B - color1.B) * amount;
        color1.A = color1.A + (color2.A - color1.A) * amount;
        return color1;
    }


    // Methods.
    public void InvertRGB()
    {
        R -= (byte.MaxValue - byte.MaxValue * R);
        G -= (byte.MaxValue - byte.MaxValue * G);
        B -= (byte.MaxValue - byte.MaxValue * B);
    }


    // Operators.
    public static FloatColor operator +(FloatColor color, float value)
    {
        color.R += value;
        color.G += value;
        color.B += value;
        color.A += value;
        return color;
    }

    public static FloatColor operator +(FloatColor color1, FloatColor color2)
    {
        color1.R += color2.R;
        color1.G += color2.G;
        color1.B += color2.B;
        color1.A += color2.A;
        return color1;
    }

    public static FloatColor operator -(FloatColor color, float value)
    {
        color.R -= value;
        color.G -= value;
        color.B -= value;
        color.A -= value;
        return color;
    }

    public static FloatColor operator -(FloatColor color1, FloatColor color2)
    {
        color1.R -= color2.R;
        color1.G -= color2.G;
        color1.B -= color2.B;
        color1.A -= color2.A;
        return color1;
    }

    public static FloatColor operator -(FloatColor color)
    {
        color.InvertRGB();
        color.A -= (byte.MaxValue - byte.MaxValue * color.A);
        return color;
    }

    public static FloatColor operator++(FloatColor color)
    {
        color.R += STEP;
        color.G += STEP;
        color.B += STEP;
        color.A += STEP;
        return color;
    }

    public static FloatColor operator --(FloatColor color)
    {
        color.R -= STEP;
        color.G -= STEP;
        color.B -= STEP;
        color.A -= STEP;
        return color;
    }

    public static FloatColor operator *(FloatColor color1, FloatColor color2)
    {
        color1.R *= color2.R;
        color1.G *= color2.G;
        color1.B *= color2.B;
        color1.A *= color2.A;
        return color1;
    }

    public static FloatColor operator *(FloatColor color, float scale)
    {
        color.R *= scale;
        color.G *= scale;
        color.B *= scale;
        color.A *= scale;
        return color;
    }

    public static implicit operator FloatColor(Color color) => new(color);

    public static implicit operator Color(FloatColor color) => new(color.R, color.G, color.B, color.A);
}