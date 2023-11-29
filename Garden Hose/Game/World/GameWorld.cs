using GardenHose.Game.World.Entities;
using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Material;
using GardenHoseEngine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Logging;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

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

    internal ReadOnlySpan<PhysicalEntity> PhysicalEntities => CollectionsMarshal.AsSpan(_physicalEntities);

    internal ReadOnlySpan<Entity> EntitiesCreated => CollectionsMarshal.AsSpan(_entitiesCreated);

    internal ReadOnlySpan<Entity> EntitiesRemoved => CollectionsMarshal.AsSpan(_entitiesRemoved);

    


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
            CameraCenter = _cameraCenter; // Forces object visual position update.
        }
    }


    /* Simulation. */
    internal float PassedTimeSeconds { get; set; }


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
    /* Layers. */
    private readonly ILayer _bottomLayer;
    private readonly ILayer _topLayer;

    /* Entities. */
    private ulong _availableID = 1;
    private readonly List<Entity> _livingEntities = new(); // For reasons decided to store entities in list instead of dictionary.
    private readonly List<Entity> _entitiesCreated = new();
    private readonly List<Entity> _entitiesRemoved = new();
    private readonly List<PhysicalEntity> _physicalEntities = new();

    /* Camera. */
    private Vector2 _cameraCenter;
    private float _zoom = 1f;

    /* Debug. */
    private bool _isDebugInfoEnabled = false;

    /* Threads. */
    private const int THREAD_COUNT = 8;
    private readonly AutoResetEvent[] _startFireEvent = new AutoResetEvent[THREAD_COUNT];
    private readonly AutoResetEvent[] _endFireEvent = new AutoResetEvent[THREAD_COUNT];
    private readonly Range[] _threadHandlingRanges =new Range[THREAD_COUNT];
    private readonly CancellationTokenSource _threadCanellationSource = new();

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
        for (int i = 0; i < THREAD_COUNT; i++)
        {
            int ThreadID = i;
            _startFireEvent[i] = new(false);
            _endFireEvent[i] = new(false);
            Task.Factory.StartNew(() => WorldThreadTask(ThreadID, _threadCanellationSource.Token), TaskCreationOptions.LongRunning);
        }
    }

    internal void End()
    {
        _threadCanellationSource.Cancel();
        for (int i = 0; i < THREAD_COUNT; i++)
        {
            _startFireEvent[i].Set();
        }
        WaitForAllThreads();
    }

    internal void Tick()
    {
        // Create and remove entities.
        if (_entitiesCreated.Count > 0)
        {
            AddNewEntities();
        }
        
        if (_entitiesRemoved.Count > 0)
        {
            RemoveOldEntities();
        }

        // Tick entities (Sequential).
        foreach (Entity WorldEntity in _livingEntities)
        {
            if (WorldEntity.IsTicked)
            {
                WorldEntity.SequentalTick();
            }
        }

        // Tick entities (Parallel).
        DivideEntitiesAmongThreads();
        FireAllThreads();
        WaitForAllThreads();

        // Test collision for physical entities.        
        DivideWorldPartsAmongThreads();
        FireAllThreads();
        WaitForAllThreads();

        // Handle collision cases, hard to do in parallel so it is kept sequential.
        foreach (var CaseCollection in _collisionCases)
        {
            HandleEntityCollisionCases(CaseCollection);
        }
        _collisionCases.Clear();
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
            Planet.Load(Game.AssetManager);
            AddEntity(Planet);
        }

        // Properties.
        AmbientMaterial = settings.AmbientMaterial?.CreateInstance() ?? throw new ArgumentNullException(nameof(settings.AmbientMaterial));
        AmbientMaterial.Temperature = 0.03f;

        // Entities.
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

    /* Tick. */
    private void HandleEntityCollisionCases(CollisionCase[] collisionCases)
    {
        foreach (CollisionCase Case in collisionCases)
        {
            if (Case.PartA.MaterialInstance.State == WorldMaterialState.Solid 
                && Case.PartB.MaterialInstance.State == WorldMaterialState.Solid)
            {
                if (Case.EntityA.Mass > Case.EntityB.Mass && Case.EntityB.IsCollisionReactionEnabled)
                {
                    Case.EntityB.PushOutOfOtherEntity(Case.BoundB, Case.BoundA, Case.EntityA, Case.PartB, Case.PartA);
                }
                else if (Case.EntityA.IsCollisionReactionEnabled)
                {
                    Case.EntityA.PushOutOfOtherEntity(Case.BoundA, Case.BoundB, Case.EntityB, Case.PartA, Case.PartB);
                }
            }

            Case.EntityA.OnCollision(Case.EntityB, Case.PartA, Case.PartB, Case.BoundA,
            Case.BoundB, Case.SurfaceNormal, Case.AverageCollisionPoint);

            Case.EntityB.OnCollision(Case.EntityA, Case.PartB, Case.PartA, Case.BoundB,
            Case.BoundA, Case.SurfaceNormal, Case.AverageCollisionPoint);
        }
    }

    private void AddNewEntities()
    {
        foreach (Entity WorldEntity in _entitiesCreated)
        {
            _livingEntities.Add(WorldEntity);

            if (!WorldEntity.IsPhysical)
            {
                continue;
            }

            PhysicalEntity PhysicalWorldEntity = (PhysicalEntity)WorldEntity;
            PhysicalWorldEntity.IsDebugInfoDrawn = IsDebugInfoEnabled;
            _physicalEntities.Add(PhysicalWorldEntity);
        }
        _entitiesCreated.Clear();
    }

    private void RemoveOldEntities()
    {
        foreach (Entity WorldEntity in _entitiesRemoved)
        {
            _livingEntities.Remove(WorldEntity);

            if (!WorldEntity.IsPhysical)
            {
                continue;
            }

            PhysicalEntity PhysicalWorldEntity = (PhysicalEntity)WorldEntity;
            _physicalEntities.Remove(PhysicalWorldEntity);
            _topLayer.RemoveDrawableItem(PhysicalWorldEntity);
            _bottomLayer.RemoveDrawableItem(PhysicalWorldEntity);
        }
        _entitiesRemoved.Clear();
    }


    /* Threads. */
    private void FireAllThreads()
    {
        foreach (AutoResetEvent Event in _startFireEvent)
        {
            Event.Set();    
        }
    }

    private void WaitForAllThreads()
    {
        foreach (AutoResetEvent Event in _endFireEvent)
        {
#if DEBUG
            Event.WaitOne();
#else
            if (!Event.WaitOne(10_000))
            {
                throw new Exception("Thread took too long to respond, aborting simulation.");
            }
#endif
        }
    }

    private void DivideWorldPartsAmongThreads()
    {
        int RowsPerThread = Math.Max(1, WORLD_PART_COUNT / THREAD_COUNT);
        int LeftoverRows = Math.Max(0, WORLD_PART_COUNT - (RowsPerThread * THREAD_COUNT));
        int ThreadIndex;
        int RowIndex;

        // First thread.
        RowIndex = RowsPerThread + LeftoverRows;
        _threadHandlingRanges[0] = new(0, RowIndex);

        // Remaining threads.
        for (ThreadIndex = 1; (ThreadIndex < THREAD_COUNT); ThreadIndex++)
        {
            int UpperRange = Math.Min(RowIndex + RowsPerThread, WORLD_PART_COUNT);
            _threadHandlingRanges[ThreadIndex] = new(RowIndex, UpperRange);
            RowIndex = UpperRange;
        }
    }

    private void DivideEntitiesAmongThreads()
    {
        int EntitiesPerThread = Math.Max(1, _livingEntities.Count / THREAD_COUNT);
        int LeftoverEntities = Math.Max(0, _livingEntities.Count - (EntitiesPerThread * THREAD_COUNT));
        int ThreadIndex;
        int EntityIndex;

        // First thread.
        EntityIndex = EntitiesPerThread + LeftoverEntities;
        _threadHandlingRanges[0] = new(0, EntityIndex);

        // Remaining threads.
        for (ThreadIndex = 1; (ThreadIndex < THREAD_COUNT); ThreadIndex++)
        {
            int UpperRange = Math.Min(EntityIndex + EntitiesPerThread, _livingEntities.Count);
            _threadHandlingRanges[ThreadIndex] = new(EntityIndex, UpperRange);
            EntityIndex = UpperRange;
        }
    }

    private void DivideCollisionsAmongThreads()
    {
        int CollisionsPerThread = Math.Max(1, _collisionCases.Count / THREAD_COUNT);
        int LeftoverEntities = Math.Max(0, _collisionCases.Count - (CollisionsPerThread * THREAD_COUNT));
        int ThreadIndex;
        int CollisionIndex;

        // First thread.
        CollisionIndex = CollisionsPerThread + LeftoverEntities;
        _threadHandlingRanges[0] = new(0, CollisionIndex);

        // Remaining threads.
        for (ThreadIndex = 1; (ThreadIndex < THREAD_COUNT); ThreadIndex++)
        {
            int UpperRange = Math.Min(CollisionIndex + CollisionsPerThread, _collisionCases.Count);
            _threadHandlingRanges[ThreadIndex] = new(CollisionIndex, UpperRange);
            CollisionIndex = UpperRange;
        }
    }

    private void WorldThreadTask(int assignedID, CancellationToken token)
    {
        try
        {
            Logger.Info($"Started world thread with ID {Thread.CurrentThread.ManagedThreadId}");

            AutoResetEvent ThisThreadEvent = _startFireEvent[assignedID];
            AutoResetEvent MainThreadEvent = _endFireEvent[assignedID];

            while (!token.IsCancellationRequested)
            {
                ThisThreadEvent.WaitOne();
                if (token.IsCancellationRequested)
                {
                    MainThreadEvent.Set();
                    return;
                }

                // Parallel entity tick.
                for (int i = _threadHandlingRanges[assignedID].Start.Value; i < _threadHandlingRanges[assignedID].End.Value; i++)
                {
                    if (_livingEntities[i].IsTicked)
                    {
                        _livingEntities[i].ParallelTick();
                    }
                }
                MainThreadEvent.Set();

                // Collision detection.
                ThisThreadEvent.WaitOne();
                TestCollisionInRows(_threadHandlingRanges[assignedID]);
                MainThreadEvent.Set();
            }
        }
        catch (Exception e)
        {
            GameFrameManager.EnqueueAction(() => throw new Exception($"Exception in physics simulation thread. {e}"));
        }

        Logger.Info($"Stopped world thread with ID {Thread.CurrentThread.ManagedThreadId}");
    }

    private void TestCollisionInRows(Range rowRange)
    {
        for (int Row = rowRange.Start.Value; Row < rowRange.End.Value; Row++)
        {
            for (int Column = 0; Column < WORLD_PART_COUNT; Column++)
            {
                TestCollisionInWorldPart(_worldParts[Row, Column]);
                _worldParts[Row, Column].Clear();
            }
        }
    }

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
            throw new Exception("Ran out of entity IDs");
        }
    }
}