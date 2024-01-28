using GardenHoseEngine.Frame;

namespace GardenHoseEngine;

public interface ITimeUpdatable
{
    public void Update(IProgramTime time);
}