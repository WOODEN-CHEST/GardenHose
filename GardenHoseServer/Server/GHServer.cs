using System;
using System.Collections.Concurrent;
using GardenHose.Messages.Server;
using GardenHose.Messages.Client;
using GardenHoseServer.World;
using System.Diagnostics;
using GardenHoseServer.Messages.Reader;
using GardenHoseServer.Messages.Collector;

namespace GardenHose.Server;

public class GHServer
{
    // Fields
    public const double SecondsPerTick = 0.05d;

    public float SimulationSpeed
    {
        get => _simulationSpeed;
        set
        {
            lock (this)
            {
                _simulationSpeed = Math.Clamp(value, 0f, 10f);
            }
        }
    }

    public bool IsPaused
    {
        get => _isPaused;
        set
        {
            lock (this)
            {
                _isPaused = value;
            }
        }
    }

    public bool IsRunning => _serverTickTask.Status == TaskStatus.Running;

    public bool IsRunningSlowly { get; private set; } = false;


    // Private fields.
    /* Ticking. */
    private Task _serverTickTask;
    private bool _isPaused = false;
    private CancellationTokenSource _cancellationTokenSource = new();

    private float _simulationSpeed = 1f;
    private Stopwatch _tickTimeMeasurer = new();
    private float _passedTimeSeconds = 0f;
    private const float MAXIMUM_PASSED_TIME_SECONDS = 0.05f;

    /*  World. */
    GameWorld _world = new();
    MessageWriter _messageWriter = new();
    MessageReader _messageReader = new();


    // Constructors.
    public GHServer()
    {
        _serverTickTask = Task.Factory.StartNew(ServerTask, 
            _cancellationTokenSource.Token, 
            TaskCreationOptions.LongRunning, 
            TaskScheduler.Default);
    }


    // Methods.
    /* Flow control. */
    public void Start()
    {
        if (IsRunning)
        {
            return;
        }

        _tickTimeMeasurer.Start();
        _serverTickTask.Start();
    }

    public void Stop()
    {
        if (!IsRunning)
        {
            return;
        }

        _cancellationTokenSource.Cancel();
    }

    /* Messages. */
    public void PostClientMessage(ClientMessage message)
    {
        _messageReader.PostClientMessage(message);
    }

    public bool ReadServerMessages(out ServerMessage[]? messages)
    {
        return _messageWriter.GetMessages(out messages);
    }


    // Private methods.
    /* Ticking. */
    private void ServerTask()
    {
        try
        {
            _world.OnStart();
            _messageWriter.CreateMessages(_world);

            Tick();

            _world.OnEnd();
        }
        catch (Exception e)
        {

        }
    }

    private void Tick()
    {
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            if (!IsRunning || IsPaused)
            {
                return;
            }

            _passedTimeSeconds = (float)_tickTimeMeasurer.Elapsed.TotalSeconds;
            _tickTimeMeasurer.Restart();

            IsRunningSlowly = (_passedTimeSeconds > MAXIMUM_PASSED_TIME_SECONDS);
            if (IsRunningSlowly)
            {
                _passedTimeSeconds = MAXIMUM_PASSED_TIME_SECONDS;
            }

            _world.Tick(_passedTimeSeconds);
            _messageWriter.CreateMessages(_world);
        }
    }
}