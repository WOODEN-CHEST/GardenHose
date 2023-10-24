﻿using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Item;
using GardenHose.Game.World;
using Microsoft.Xna.Framework.Graphics;
using GardenHoseEngine.Frame.Item.Basic;
using GardenHoseEngine;
using Microsoft.Xna.Framework;
using System.Drawing;
using NAudio.CoreAudioApi;

namespace GardenHose.Game.World.Entities;

internal abstract class DrawablePhysicalEntity : PhysicalEntity, IDrawableItem
{
    // Fields.
    public virtual bool IsVisible { get; set; } = true;

    public virtual Effect? Shader { get; set; }


    // Internal fields.
    internal virtual DrawLayer Layer { get; set; } = DrawLayer.Bottom;

    internal bool DrawCollisionBox = false;


    // Protected fields.
    protected Line VisualLine { get; private init; } = new() { Thickness = 10f };


    // Constructors.
    public DrawablePhysicalEntity(EntityType type, GameWorld? world, PhysicalEntityPart mainPart) 
        : base(type, world, mainPart) { }


    // Protected methods.
    protected virtual void DrawCollisionBounds()
    {
        if (MainPart == null) return;

        void DrawPart(PhysicalEntityPart part)
        {
            foreach (ICollisionBound Bound in part.CollisionBounds)
            {
                Bound.Draw(part.Position, part.FullRotation, VisualLine, World!);
            }

            if (part.LinkedParts == null) return;

            foreach (PartLink Link in part.LinkedParts)
            {
                DrawPart(Link.LinkedPart);
            }
        }

        DrawPart(MainPart);
    }



    // Inherited methods.
    public virtual void Draw()
    {
        if (DrawCollisionBox)
        {
            DrawCollisionBounds();
        }
    }
}
