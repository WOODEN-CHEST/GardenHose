using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseServer.World.Entities;

internal interface IIDProvider
{
    public ulong GetID();
}