using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GardenHose.Messages.Server;

public enum ServerMessageType
{
    ObjectStateUpdate,
    ObjectCreation,
    ObjectDeletion,
    Special,
    Exception
}