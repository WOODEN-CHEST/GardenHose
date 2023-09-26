using GardenHoseServer.World.Entities;
using GardenHoseServer.World.Physics;

namespace GardenHoseServer.World;

public class GameWorld : IIDProvider
{
    // Internal fields.
    internal PhysicsEngine PhysicsEngine { get; private init; } = new();

    internal Planet Planet { get; private init; } = new(200, 9.81f);

    internal IEnumerable<Entity> Entitites => _entities.Values;

    internal IEnumerable<Entity> EntitiesCreated => _entitiesCreated;

    internal IEnumerable<Entity> EntitiesDeleted => _entitiesRemoved;



    // Private fields
    /* Entities. */
    private Dictionary<ulong, Entity> _entities = new();
    private readonly List<Entity> _entitiesCreated = new();
    private readonly List<Entity> _entitiesRemoved = new();
    private ulong _availableID = 0;


    // Constructors.
    internal GameWorld()
    {
        //AddEntity(new TestEntity(GetID(), EntityType.Test, this) { Position = new(400f, 0f) });
    }


    // Methods.
    /* Flow control. */
    internal void Tick(float passedTimeSeconds)
    {
        // Create and remove entities.
        foreach (Entity WorldEntity in _entitiesCreated)
        {
            _entities.Add(WorldEntity.ID, WorldEntity);
        }
        _entitiesCreated.Clear();

        foreach(Entity WorldEntity in _entitiesRemoved)
        {
            _entities.Remove(WorldEntity.ID);
        }
        _entitiesRemoved.Clear();

        // Tick entities.
        foreach (Entity WorldEntity in _entities.Values)
        {
            WorldEntity.Tick(passedTimeSeconds);
        }
    }

    internal void OnStart()
    {
        AddEntity(new TestEntity(this) { Position = new(400, 300)});
    }

    internal void OnEnd()
    {

    }

    /* Entities. */
    internal void AddEntity(Entity entity)
    {
        if (_entities.ContainsKey(entity.ID))
        {
            throw new InvalidOperationException($"Duplicate entity ID: {entity.ID}");
        }

        _entitiesCreated.Add(entity);
    }

    internal void RemoveEntity(Entity entity)
    {
        _entitiesRemoved.Add(entity);
    }

    internal Entity? GetEntity(ulong ID)
    {
        if (_entities.ContainsKey(ID))
        {
            return _entities[ID];
        }
        return null;
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