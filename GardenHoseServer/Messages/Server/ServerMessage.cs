using GardenHoseServer.Messages;

namespace GardenHose.Messages.Server;


public record class ServerMessage : Message
{
    public ServerMessageType Type;
}