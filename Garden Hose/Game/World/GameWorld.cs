using GardenHose;
using GardenHose.Game;
using GardenHose.Game.World;
using GardenHose.Game.World.Entities;
using GardenHose.Game.World.Planet;
using GardenHoseEngine.Screen;
using GardenHose.Game.World.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace GardenHose.Game.World;


public class GameWorld : IIDProvider
{
    // Internal fields.
    internal WorldPlanet Planet { get; private set; }

    internal IEnumerable<Entity> Entitites => _entities.Values;

    internal IEnumerable<Entity> EntitiesCreated => _entitiesCreated;

    internal IEnumerable<Entity> EntitiesDeleted => _entitiesRemoved;

    internal GHGame Game { get; private init; }

    internal Vector2 CameraCenter
    {
        get => _cameraCenter;
        set
        {
            _cameraCenter = value;
            ObjectVisualOffset = (Display.VirtualSize / 2f) - (_cameraCenter * Zoom);
        }
    }

    internal Vector2 ObjectVisualOffset { get; private set; }

    internal float Zoom
    {
        get => _zoom;
        set
        {
            _zoom = value;
            CameraCenter = _cameraCenter; // Forces object visual position update.
        }
    }

    internal float PassedTimeSeconds { get; set; }


    // Private fields
    /* Entities. */
    private readonly Dictionary<ulong, Entity> _entities = new();
    private readonly List<Entity> _entitiesCreated = new();
    private readonly List<Entity> _entitiesRemoved = new();
    private ulong _availableID = 1;

    /* Camera. */
    private Vector2 _cameraCenter;
    private float _zoom = 1f;


    // Constructors.
    internal GameWorld(GHGame game, GameWorldSettings settings)
    {
        Game = game ?? throw new ArgumentNullException(nameof(game));
        CameraCenter = new(0, 0);

        ReadStartupSettings(settings);
    }


    // Methods.
    /* Flow control. */
    internal void Start()
    {

    }

    internal void End()
    {

    }

    internal void Tick()
    {
        // Create and remove entities.
        foreach (Entity WorldEntity in _entitiesCreated)
        {
            _entities.Add(WorldEntity.ID, WorldEntity);
        }
        _entitiesCreated.Clear();

        foreach (Entity WorldEntity in _entitiesRemoved)
        {
            _entities.Remove(WorldEntity.ID);
        }
        _entitiesRemoved.Clear();

        // Tick entities.
        foreach (Entity WorldEntity in _entities.Values)
        {
            WorldEntity.Tick();
        }

        foreach (Entity WorldEntity in _entities.Values)
        {
            if (!WorldEntity.IsPhysical) continue;

            ((PhysicalEntity)WorldEntity).TestCollision();
        }
    }

    /* Entities. */
    internal void AddEntity(Entity entity)
    {
        entity.World = this;
        if (entity.ID == 0)
        {
            entity.ID = GetID();
        }

        if (_entities.ContainsKey(entity.ID))
        {
            throw new InvalidOperationException($"Duplicate entity ID: {entity.ID}");
        }

        PhysicalEntity? DrawableEntity = entity as PhysicalEntity;
        if (DrawableEntity != null)
        {
            if (DrawableEntity.DrawLayer == DrawLayer.Top)
            {
                Game.TopItemLayer.AddDrawableItem(DrawableEntity);
            }
            else
            {
                Game.BottomItemLayer.AddDrawableItem(DrawableEntity);
            }
        }

        entity.Load(Game.AssetManager);

        _entitiesCreated.Add(entity);
    }

    internal void RemoveEntity(Entity entity)
    {
        _entitiesRemoved.Add(entity);
    }

    internal EntityType? GetEntity<EntityType>(ulong ID) where EntityType : Entity
    {
        _entities.TryGetValue(ID, out var Entity);

        if (Entity == null)
        {
            foreach (Entity RemovedEntity in _entitiesCreated)
            {
                if (RemovedEntity.ID == ID)
                {
                    Entity = RemovedEntity;
                }
            }
        }

        return Entity as EntityType;
    }

    /* Camera. */
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Vector2 ToViewportPosition(Vector2 worldPosition)
    {
        return (worldPosition * Zoom) + ObjectVisualOffset;
    }


    // Private methods.
    [MemberNotNull(nameof(Planet))]
    private void ReadStartupSettings(GameWorldSettings settings)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        Planet = settings.Planet;
        Planet.World = this;
        Planet.Load(Game.AssetManager);
        Game.BackgroundLayer.AddDrawableItem(Planet);

        foreach (var Entity in settings.StartingEntities)
        {
            AddEntity(Entity);
        }
    }


    // Inherited methods.
    public ulong GetID()
    {
        try
        {
            checked
            {
                return _availableID++;
            }
        }
        catch (OverflowException)
        {
            throw new Exception("Ran out of entity IDs");
        }
    }
}