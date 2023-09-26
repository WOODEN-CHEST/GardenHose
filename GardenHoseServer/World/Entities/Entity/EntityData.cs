using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseServer.World.Entities;

public class EntityData
{
    // Fields.
    public ulong ID { get; }

    public EntityType Type { get; }


    // Constructors.
    internal EntityData(Entity entity)
    {
        ID = entity.ID;
        Type = entity.EntityType;
    }
}