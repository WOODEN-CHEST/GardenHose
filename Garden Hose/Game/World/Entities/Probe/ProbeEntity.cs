using GardenHose.Game.AssetManager;
using GardenHose.Game.World.Entities.Ship;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities.Probe;

internal class ProbeEntity : SpaceshipEntity
{
    // Internal fields.



    // Private fields.
    private ProbeMainPart? _mainPart;
    private ProbeHeadPart? _headPart;
    private ProbeSideThrusterPart? _leftThrusterPart;
    private ProbeSideThrusterPart? _rightThrusterPart;
    private ProbeMainThrusterPart? _mainThrusterPart;


    // Constructors.
    public ProbeEntity(EntityType type, GameWorld? world) : base(type, world)
    {
    }


    // Inherited methods.
    internal override void Load(GHGameAssetManager assetManager)
    {
        
    }

    protected override void AITick()
    {
        
    }

    protected override void TryToNavigateToLocation(Vector2 location)
    {
        
    }

    protected override void PlayerTick()
    {
        
    }

    internal override void OnPartDamage() { }

    internal override void OnPartBreak() { }
}