
namespace GardenHose.Game.World.Entities;

public enum EntityType
{
    Unknown,

    // Planet related.
    Planet,
    BuildingPlaceholder,

    // Special entities.
    Marker,
    Test,
    Stray,

    // Particles.
    Particle,

    // player ships.
    Probe,
    
    // Enemy ships.
    UFOBasic,
    UFOAdvanced,
    UFOExtraAdvanced,
    UFOSmall,
    UFOLarge,

    Ball,
    BallShooter,
    BallSpiked,
    BallSpikedShooter,
    
    SmallAttackShip,
    SmallAttackShipAdvanced
}