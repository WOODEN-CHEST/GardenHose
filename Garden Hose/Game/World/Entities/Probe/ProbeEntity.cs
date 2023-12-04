using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Events;
using GardenHose.Game.World.Entities.Ship;
using GardenHose.Game.World.Entities.Ship.System;
using GardenHoseEngine;

namespace GardenHose.Game.World.Entities.Probe;

internal class ProbeEntity : SpaceshipEntity
{
    // Internal fields.
    internal override ISpaceshipSystem ShipSystem { get; init; }

    internal const float SPRITE_SCALING = 0.217f;


    // Private fields.
    /* Parts. */
    private ProbeHeadPart? _headPart;
    private ProbeSideThrusterPart? _leftThrusterPart;
    private ProbeSideThrusterPart? _rightThrusterPart;
    private ProbeMainThrusterPart? _mainThrusterPart;

    /* System. */
    private ProbeSystem _system;



    // Constructors.
    public ProbeEntity() : base(EntityType.Probe, null)
    {
        _system = new ProbeSystem(this);
        ShipSystem = _system;

        PhysicalEntityPart BasePart = new ProbeMainPart(this);
        _headPart = new ProbeHeadPart(this);
        _leftThrusterPart = new ProbeSideThrusterPart(this, Origin.CenterLeft);
        _rightThrusterPart = new ProbeSideThrusterPart(this, Origin.CenterRight);
        _mainThrusterPart = new(this);

        // A lot of magic numbers here are just offsets which were not calculated but just eyed until it looked right.
        BasePart.LinkPart(_headPart, new(0f, (-ProbeMainPart.HitboxSize.Y * 0.5f) - (ProbeHeadPart.HitboxSize.Y * 0.5f) + 1.25f));
        BasePart.LinkPart(_rightThrusterPart, new(ProbeMainPart.HitboxSize.X * 0.5f + ProbeSideThrusterPart.HitboxSize.X * 0.5f - 2.5f, 0f));
        BasePart.LinkPart(_leftThrusterPart, -_rightThrusterPart.ParentLink!.LinkDistance);
        BasePart.LinkPart(_mainThrusterPart, new(0f, ProbeMainPart.HitboxSize.Y * 0.5f + ProbeMainThrusterPart.HitboxSize.X * 0.5f - 13.5f));

        MainPart = BasePart;
    }

    

    protected override void AIParallelTick()
    {

    }

    protected override void AISequentialTick()
    {

    }

    protected override void PlayerParallelTick()
    {

    }

    protected override void PlayerSequentialTick()
    {

    }

    internal override void OnPartDamage(PartDamageEventArgs args)
    {

    }

    internal override void OnPartDestroy(PartDamageEventArgs args)
    {

    }


    // Inherited methods.
}