using GardenHose.Game.World.Entities;
using GardenHose.Game.World.Entities.Physical;
using GardenHoseEngine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GardenHose.Game.World;


public class GameWorld : IIDProvider
{
    // Internal fields.
    internal WorldPlanet? Planet { get; private set; }

    internal IEnumerable<Entity> Entities => _livingEntities.Concat(_entitiesCreated);

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

    internal bool IsDebugInfoEnabled
    {
        get => _isDebugInfoEnabled;
        set
        {
            _isDebugInfoEnabled = value;
            UpdateDebugInfoVisibility();
        }
    }


    // Private fields
    /* Layers. */
    private readonly ILayer _bottomLayer;
    private readonly ILayer _topLayer;

    /* Entities. */
    private readonly List<Entity> _livingEntities = new(); // For reasons decided to store entities in list instead of dictionary.
    private readonly List<Entity> _entitiesCreated = new();
    private readonly List<Entity> _entitiesRemoved = new();

    private readonly List<PhysicalEntity> _physicalEntities = new();
    private ulong _availableID = 1;

    /* Camera. */
    private Vector2 _cameraCenter;
    private float _zoom = 1f;

    /* Debug. */
    private bool _isDebugInfoEnabled = false;


    // Constructors.
    internal GameWorld(GHGame game, ILayer bottomLayer, ILayer topLayer, GameWorldSettings settings)
    {
        Game = game ?? throw new ArgumentNullException(nameof(game));
        CameraCenter = new(0, 0);

        _bottomLayer = bottomLayer ?? throw new ArgumentNullException(nameof(bottomLayer));
        _topLayer = topLayer ?? throw new ArgumentNullException(nameof(topLayer));

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
        if (_entitiesCreated.Count > 0)
        {
            foreach (Entity WorldEntity in _entitiesCreated)
            {
                _livingEntities.Add(WorldEntity);

                if (WorldEntity.IsPhysical)
                {
                    _physicalEntities.Add((PhysicalEntity)WorldEntity);
                }
            }
            _entitiesCreated.Clear();
        }
        
        if (_entitiesRemoved.Count > 0)
        {
            foreach (Entity WorldEntity in _entitiesRemoved)
            {
                _livingEntities.Remove(WorldEntity);

                if (WorldEntity.IsPhysical)
                {
                    PhysicalEntity PhysicalWorldEntity = (PhysicalEntity)WorldEntity;
                    _physicalEntities.Remove(PhysicalWorldEntity);
                    _topLayer.RemoveDrawableItem(PhysicalWorldEntity);
                    _bottomLayer.RemoveDrawableItem(PhysicalWorldEntity);
                }

                
            }
            _entitiesRemoved.Clear();
        }

        // Tick entities.
        foreach (Entity WorldEntity in _livingEntities)
        {
            WorldEntity.Tick();
        }

        // Test collision for physical entities.
        for (int FirstIndex = 0; FirstIndex < _physicalEntities.Count; FirstIndex++)
        {
            for (int SecondIndex = FirstIndex + 1; SecondIndex < _physicalEntities.Count; SecondIndex++)
            {
                if (!_physicalEntities[FirstIndex].TestCollisionAgainstEntity(
                    _physicalEntities[SecondIndex], out CollisionCase[] collisions))
                {
                    continue;
                }

                foreach (CollisionCase Case in collisions)
                {

                }
            }
        }
    }

    /* Entities. */
    internal void AddEntity(Entity entity)
    {
        // Set entity properties.
        entity.World = this;
        if (entity.ID == 0)
        {
            entity.ID = GetID();
        }

        // Add entity to drawing layer.
        PhysicalEntity? DrawableEntity = entity as PhysicalEntity;
        if (DrawableEntity != null)
        {
            if (DrawableEntity.DrawLayer == DrawLayer.Top)
            {
                _topLayer.AddDrawableItem(DrawableEntity);
            }
            else if (DrawableEntity.DrawLayer == DrawLayer.Bottom)
            {
                _bottomLayer.AddDrawableItem(DrawableEntity);
            }
            else
            {
                throw new EnumValueException(nameof(DrawableEntity.DrawLayer), nameof(DrawLayer),
                    DrawableEntity.DrawLayer.ToString(), (int)DrawableEntity.DrawLayer);
            }
            DrawableEntity.IsDebugInfoDrawn = IsDebugInfoEnabled;
        }

        // Load entity's assets.
        entity.Load(Game.AssetManager);

        // Finally add entity to the world.
        _entitiesCreated.Add(entity);
    }

    internal void RemoveEntity(Entity entity)
    {
        _entitiesRemoved.Add(entity);
    }

    internal EntityType? GetEntity<EntityType>(ulong id) where EntityType : Entity
    {
        Entity? WorldEntity = null;

        foreach (Entity AddedEntity in _livingEntities)
        {
            if (AddedEntity.ID == id)
            {
                WorldEntity = AddedEntity;
                break;
            }
        }

        if (WorldEntity == null)
        {
            foreach (Entity AddedEntity in _entitiesCreated)
            {
                if (AddedEntity.ID == id)
                {
                    WorldEntity = AddedEntity;
                    break;
                }
            }
        }

        return WorldEntity as EntityType;
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

        /* Entities. */
        if (settings.Planet != null)
        {
            Planet = settings.Planet;
            Planet.World = this;
            Planet.Load(Game.AssetManager);
            AddEntity(Planet);
        }

        foreach (var Entity in settings.StartingEntities)
        {
            AddEntity(Entity);
        }
    }

    private void UpdateDebugInfoVisibility()
    {
        foreach (PhysicalEntity WorldEntity in _physicalEntities)
        {
            WorldEntity.IsDebugInfoDrawn = IsDebugInfoEnabled;
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