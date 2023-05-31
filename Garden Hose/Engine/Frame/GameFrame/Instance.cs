﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using GardenHose.Engine.IO;
using GardenHose.Engine.Frame.UI.Item;
using GardenHose.Engine.Frame.UI.Animation;
using Microsoft.Xna.Framework.Audio;
using System.Numerics;

namespace GardenHose.Engine.Frame;


public partial class GameFrame
{
    // Fields.
    public readonly List<Layer> Layers = new();
    public readonly HashSet<IUpdateableItem> UpdateableItems = new();
    

    public string Name
    {
        get => name;
        set
        {
            if (String.IsNullOrEmpty(value)) throw new ArgumentException(nameof(value));
            name = value;
        }
    }


    // Protected fields.
    protected string name = "Default Frame Name";


    // Private fields.
    private readonly List<FrameAnimation> _animations = new();
    private readonly List<FrameSound> _sounds = new();

    private bool isInitialized = false;


    // Constructors.
    public GameFrame(string name) 
    {
        Name = name;
    }


    // Methods.

    /* Loops */
    public virtual void Update()
    {
        foreach (var Item in UpdateableItems) Item.Update();
    }

    public virtual void Draw()
    {
        if (Layers.Count == 0) return;

        Effect Shader = null;
        bool BatchStarted = false;

        foreach (Layer L in Layers)
        {
            // Set the shader if needed.
            if (Shader != L.Shader)
            {
                if (BatchStarted) DrawBatch.End();
                BatchStarted = false;
            }
            if (!BatchStarted)
            {
                Shader = L.Shader;
                DrawBatch.Begin(SpriteSortMode.Deferred,
                    BlendState.NonPremultiplied,
                    SamplerState.LinearClamp,
                    DepthStencilState.None,
                    RasterizerState.CullCounterClockwise,
                    Shader);
                BatchStarted = true;
            }

            // Draw.
            L.Draw();
        }

        DrawBatch.End();
    }

    public virtual void Restart()
    {
        OnStart();
    }


    // Protected methods.
    /* Management */
    protected FrameAnimation CreateAnimation(Func<Animation> animationCreator)
    {
        FrameAnimation Anim = new FrameAnimation(animationCreator);
        _animations.Add(Anim);
        return Anim;
    }

    protected FrameSound CreateSound(string relativePath)
    {
        FrameSound Sound = new FrameSound(relativePath);
        _sounds.Add(Sound);
        return Sound;
    }


    /* Events */
    protected virtual void Load()
    {

    }

    protected virtual void OnStart()
    {

    }

    protected virtual void Unload()
    {
        foreach (var Animation in _animations) Animation.Dispose();
        foreach (var Sound in _sounds) Sound.Dispose();

        foreach (var Item in UpdateableItems) Item.Dispose();
    }

    protected virtual void OnEnd()
    {

    }
}