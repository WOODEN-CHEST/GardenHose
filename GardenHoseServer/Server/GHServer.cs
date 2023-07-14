using System;
using System.Collections.Concurrent;
using GardenHose.Messages.Server;
using GardenHose.Messages.Client;


namespace GardenHose.Server;

public class GHServer
{
    // Static fields.


    // Fields.
    public bool IsTicking
    {
        get => _isTicking;
        set
        {
            if (!_wasStarted)
            {
                throw new ArgumentException("Server has not been started.");
            }
            else if (_isTicking == value) return;

            _isTicking = value;

            if (_isTicking)
            {
                _tickTimer = new(OnTick, null, 50, 50);
            }
            else _tickTimer.Dispose();
        }
    }


    // Private fields.
    private readonly ConcurrentQueue<ClientMessage> _clientMessages = new();
    private readonly ConcurrentQueue<ServerMessage> _serverMessages = new();

    private Timer _tickTimer;
    private bool _isTicking = true;
    private bool _wasStarted;


    // Constructors.


    // Static methods.


    // Methods.
    public void PostClientMessage(ClientMessage msg) => _clientMessages.Enqueue(msg);

    public bool ReadServerMessage(out ServerMessage msg) => _serverMessages.TryDequeue(out msg);


    // Private methods.
    private void OnTick(object state)
    {

    }

    private void ReadMessages()
    {
        while (_clientMessages.TryDequeue(out ClientMessage Msg))
        {
            throw new NotImplementedException();
        }
    }

    private void Start()
    {
        _wasStarted = true;
        IsTicking = true;
    }

    private void End()
    {
        _tickTimer.Dispose();
    }
}