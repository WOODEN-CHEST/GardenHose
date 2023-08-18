using GardenHose.Messages.Server;
using GardenHoseServer.Messages;

namespace GardenHose.Messages.Client;


public record class ClientMessage : Message
{
    public ServerMessageType Type;
}