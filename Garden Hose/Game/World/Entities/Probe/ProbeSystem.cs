using GardenHose.Game.World.Entities.Ship;
using GardenHose.Game.World.Entities.Ship.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GardenHose.Game.World.Entities.Probe;

internal class ProbeSystem : ISpaceshipSystem
{
    // Fields.
    public bool IsEnabled { get; set; } = true;

    public SpaceshipEntity Ship => _probe;

    public bool IsVisible { get; set; } = true;
    public Effect? Shader { get; set; }


    // Private fields.
    private ProbeEntity _probe;



    // Constructors.
    internal ProbeSystem(ProbeEntity probe)
    {
        _probe = probe ?? throw new ArgumentNullException(nameof(probe));
    }


    // Inherited methods.
    /* Drawing. */
    public void Draw()
    {
        if (!IsVisible) return;
    }

    /* Navigating. */
    public void NavigateToPosition(Vector2 position)
    {
        
    }

    /* Tick. */
    public void ParallelTick()
    {
        
    }

    public void SequentialTick()
    {
        
    }
}