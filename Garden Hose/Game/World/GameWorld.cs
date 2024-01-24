﻿using GardenHose.Game.World.Entities;
using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;
using GardenHose.Game.World.Entities.Planet;
using GardenHose.Game.World.Material;
using GardenHose.Game.World.Player;
using GardenHoseEngine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
// Import everything.

namespace GardenHose.Game.World;


public class GameWorld : IIDProvider
{
    // Internal fields.
    internal GHGame Game { get; private init; }


    /* World components */
    internal WorldPlanetEntity? Planet { get; private set; }


    /* Properties. */
    internal WorldMaterialInstance AmbientMaterial { get; private set; }


    /* Entities. */
    internal int EntityCount => _livingEntities.Count + _entitiesCreated.Count;
    internal IEnumerable<Entity> Entities => _livingEntities.Concat(_entitiesCreated);
    internal ReadOnlySpan<Entity> LivingEntities => CollectionsMarshal.AsSpan(_livingEntities);
    internal ReadOnlySpan<Entity> EntitiesCreated => CollectionsMarshal.AsSpan(_entitiesCreated);
    internal ReadOnlySpan<Entity> EntitiesRemoved => CollectionsMarshal.AsSpan(_entitiesRemoved);


    /* Player. */
    internal WorldPlayer Player { get; private set; }


    /* Camera. */
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
            InverseZoom = 1f / value;
            CameraCenter = _cameraCenter; // Forces object visual position update.
        }
    }

    internal float InverseZoom { get; private set; } = 1f;


    /* Debug. */
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
    /* Drawing Layers. */
    private readonly ILayer _bottomLayer;
    private readonly ILayer _topLayer;

    /* Entities. */
    private ulong _availableID = 1;
    private readonly List<Entity> _livingEntities = new();
    private readonly List<Entity> _entitiesCreated = new();
    private readonly List<Entity> _entitiesRemoved = new();

    /* Camera. */
    private Vector2 _cameraCenter;
    private float _zoom = 1f;

    /* Debug. */
    private bool _isDebugInfoEnabled = false;

    /* Collisions. */
    private ConcurrentQueue<CollisionCase[]> _collisionCases = new();
    private const int WORLD_PART_POWER = 7;
    private const int WORLD_PART_SIZE = 128; // 2^7.
    private const int WORLD_PART_COUNT = 32;
    private readonly List<PhysicalEntity>[,] _worldParts = new List<PhysicalEntity>[WORLD_PART_COUNT, WORLD_PART_COUNT];
    


    // Constructors.
    internal GameWorld(GHGame game, ILayer bottomLayer, ILayer topLayer, GameWorldSettings settings)
    {
        Game = game ?? throw new ArgumentNullException(nameof(game));
        CameraCenter = new(0, 0);

        _bottomLayer = bottomLayer ?? throw new ArgumentNullException(nameof(bottomLayer));
        _topLayer = topLayer ?? throw new ArgumentNullException(nameof(topLayer));

        for (int i = 0; i < WORLD_PART_COUNT; i++)
        {
            for (int j = 0; j < WORLD_PART_COUNT; j++)
            {
                _worldParts[i, j] = new();
            }
        }
        
        ReadStartupSettings(settings);
    }


    // Methods.
    /* Flow control. */
    internal void Start()
    {
        AddNewEntities();
    }

    internal void End() { }

    internal void Tick(GHGameTime gameTime)
    {
        // Tick.
        foreach (Entity WorldEntity in _livingEntities)
        {
            if (WorldEntity.IsTicked)
            {
                WorldEntity.Tick(gameTime);
            }
        }
        Player.Tick();


        // Handle collisions.
        for (int Row = 0; Row < WORLD_PART_COUNT; Row++)
        {
            for (int Column = 0;  Column < WORLD_PART_COUNT; Column++)
            {
                TestCollisionInWorldPart(_worldParts[Row, Column]);
                _worldParts[Row, Column].Clear();
            }
        }

        foreach (var CaseCollection in _collisionCases)
        {
            HandleEntityCollisionCases(CaseCollection);
        }
        _collisionCases.Clear();

        // Create and remove entities.
        AddNewEntities();
        RemoveOldEntities();
    }

    /* Entities. */
    internal void AddEntity(Entity entity)
    {
        // Set entity properties.«
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Vector2 ToWorldPosition(Vector2 viewportPosition)
    {
        return (viewportPosition - ObjectVisualOffset) / Zoom;
    }


    /* Collision. */
    internal void AddPhysicalEntityToWorldPart(PhysicalEntity entity)
    {
        int HALF_WORLD_PART_COUNT = WORLD_PART_COUNT / 2;
        int LowerX = Math.Max(0, ((int)(entity.Position.X - entity.BoundingLength) >> WORLD_PART_POWER) + HALF_WORLD_PART_COUNT);
        int LowerY = Math.Max(0, ((int)(entity.Position.Y - entity.BoundingLength) >> WORLD_PART_POWER) + HALF_WORLD_PART_COUNT);

        int UpperX = Math.Min(WORLD_PART_COUNT - 1,
            ((int)(entity.Position.X + entity.BoundingLength) >> WORLD_PART_POWER) + HALF_WORLD_PART_COUNT);
        int UpperY = Math.Min(WORLD_PART_COUNT - 1,
            ((int)(entity.Position.Y + entity.BoundingLength) >> WORLD_PART_POWER) + HALF_WORLD_PART_COUNT);

        for (int i = LowerX; i <= UpperX; i++)
        {
            for (int j = LowerY; j <= UpperY; j++)
            {
                _worldParts[i,j].Add(entity);
            }
        }
    }


    // Private methods.
    [MemberNotNull(nameof(Planet))]
    private void ReadStartupSettings(GameWorldSettings settings)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        // Components
        if (settings.Planet != null)
        {
            Planet = settings.Planet;
            Planet.World = this;
            AddEntity(Planet);
        }

        // Properties.
        AmbientMaterial = settings.AmbientMaterial?.CreateInstance() ?? throw new ArgumentNullException(nameof(settings.AmbientMaterial));
        AmbientMaterial.Temperature = 0.03f;
        
        // Entities.
        AddEntity(settings.PlayerShip);
        Player = new(this, settings.PlayerShip);

        foreach (var Entity in settings.StartingEntities)
        {
            AddEntity(Entity);
        }
    }

    private void UpdateDebugInfoVisibility()
    {
        foreach (Entity WorldEntity in _livingEntities)
        {
            if (WorldEntity.IsPhysical)
            {
                ((PhysicalEntity)WorldEntity).IsDebugInfoDrawn = IsDebugInfoEnabled;
            }
        }
    }

    /* Tick. */
    private void HandleEntityCollisionCases(CollisionCase[] collisionCases)
    {
        foreach (CollisionCase Case in collisionCases)
        {
            if (Case.EntityA.Mass > Case.EntityB.Mass && Case.EntityB.IsCollisionReactionEnabled)
            {
                Case.EntityB.PushOutOfOtherEntity(Case.BoundB, Case.BoundA, Case.EntityA, Case.PartB, Case.PartA);
            }
            else if (Case.EntityA.IsCollisionReactionEnabled)
            {
                Case.EntityA.PushOutOfOtherEntity(Case.BoundA, Case.BoundB, Case.EntityB, Case.PartA, Case.PartB);
            }

            Case.EntityA.OnCollision(Case.EntityB,
                Case.PartA,
                Case.PartB,
                Case.EntityA.Motion,
                Case.EntityB.Motion,
                Case.EntityARotationalMotionAtPoint,
                Case.EntityBRotationalMotionAtPoint,
                Case.SurfaceNormal,
                Case.AverageCollisionPoint);

            Case.EntityB.OnCollision(Case.EntityA,
                Case.PartB,
                Case.PartA,
                Case.EntityB.Motion,
                Case.EntityA.Motion,
                Case.EntityBRotationalMotionAtPoint,
                Case.EntityARotationalMotionAtPoint,
                Case.SurfaceNormal,
                Case.AverageCollisionPoint);
        }
    }

    private void AddNewEntities()
    {
        if (_entitiesCreated.Count == 0)
        {
            return;
        }

        foreach (Entity WorldEntity in _entitiesCreated)
        {
            _livingEntities.Add(WorldEntity);

            if (!WorldEntity.IsPhysical)
            {
                continue;
            }

            PhysicalEntity PhysicalWorldEntity = (PhysicalEntity)WorldEntity;
            PhysicalWorldEntity.IsDebugInfoDrawn = IsDebugInfoEnabled;
        }
        _entitiesCreated.Clear();
    }

    private void RemoveOldEntities()
    {
        if (_entitiesRemoved.Count == 0)
        {
            return;
        }

        foreach (Entity WorldEntity in _entitiesRemoved)
        {
            _livingEntities.Remove(WorldEntity);

            if (!WorldEntity.IsPhysical)
            {
                continue;
            }

            PhysicalEntity PhysicalWorldEntity = (PhysicalEntity)WorldEntity;
            _topLayer.RemoveDrawableItem(PhysicalWorldEntity);
            _bottomLayer.RemoveDrawableItem(PhysicalWorldEntity);
        }
        _entitiesRemoved.Clear();
    }

    //private void DivideWorldPartsAmongThreads()
    //{
    //    int RowsPerThread = Math.Max(1, WORLD_PART_COUNT / THREAD_COUNT);
    //    int LeftoverRows = Math.Max(0, WORLD_PART_COUNT - (RowsPerThread * THREAD_COUNT));
    //    int ThreadIndex;
    //    int RowIndex;

    //    // First thread.
    //    RowIndex = RowsPerThread + LeftoverRows;
    //    _threadHandlingRanges[0] = new(0, RowIndex);

    //    // Remaining threads.
    //    for (ThreadIndex = 1; (ThreadIndex < THREAD_COUNT); ThreadIndex++)
    //    {
    //        int UpperRange = Math.Min(RowIndex + RowsPerThread, WORLD_PART_COUNT);
    //        _threadHandlingRanges[ThreadIndex] = new(RowIndex, UpperRange);
    //        RowIndex = UpperRange;
    //    }
    //}

    //private void DivideEntitiesAmongThreads()
    //{
    //    int EntitiesPerThread = Math.Max(1, _livingEntities.Count / THREAD_COUNT);
    //    int LeftoverEntities = Math.Max(0, _livingEntities.Count - (EntitiesPerThread * THREAD_COUNT));
    //    int ThreadIndex;
    //    int EntityIndex;

    //    // First thread.
    //    EntityIndex = EntitiesPerThread + LeftoverEntities;
    //    _threadHandlingRanges[0] = new(0, EntityIndex);

    //    // Remaining threads.
    //    for (ThreadIndex = 1; (ThreadIndex < THREAD_COUNT); ThreadIndex++)
    //    {
    //        int UpperRange = Math.Min(EntityIndex + EntitiesPerThread, _livingEntities.Count);
    //        _threadHandlingRanges[ThreadIndex] = new(EntityIndex, UpperRange);
    //        EntityIndex = UpperRange;
    //    }
    //}

    //private void DivideCollisionsAmongThreads()
    //{
    //    int CollisionsPerThread = Math.Max(1, _collisionCases.Count / THREAD_COUNT);
    //    int LeftoverEntities = Math.Max(0, _collisionCases.Count - (CollisionsPerThread * THREAD_COUNT));
    //    int ThreadIndex;
    //    int CollisionIndex;

    //    // First thread.
    //    CollisionIndex = CollisionsPerThread + LeftoverEntities;
    //    _threadHandlingRanges[0] = new(0, CollisionIndex);

    //    // Remaining threads.
    //    for (ThreadIndex = 1; (ThreadIndex < THREAD_COUNT); ThreadIndex++)
    //    {
    //        int UpperRange = Math.Min(CollisionIndex + CollisionsPerThread, _collisionCases.Count);
    //        _threadHandlingRanges[ThreadIndex] = new(CollisionIndex, UpperRange);
    //        CollisionIndex = UpperRange;
    //    }
    //}


    /* Collision. */
    private void TestCollisionInWorldPart(List<PhysicalEntity> entities)
    {
        for (int FirstIndex = 0; FirstIndex < entities.Count; FirstIndex++)
        {
            for (int SecondIndex = FirstIndex + 1; SecondIndex < entities.Count; SecondIndex++)
            {
                lock (entities[FirstIndex])
                {
                    if (!entities[FirstIndex].TestCollisionAgainstEntity(
                        entities[SecondIndex], out CollisionCase[] collisions))
                    {
                        continue;
                    }

                    if (collisions.Length > 0)
                    {
                        _collisionCases.Enqueue(collisions);
                    }
                }
            }
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
            throw new Exception("Ran out of entity IDs, cannot safely continue game.");
        }
    }
}