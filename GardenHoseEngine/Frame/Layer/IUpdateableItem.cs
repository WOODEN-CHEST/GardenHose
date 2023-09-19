using System;


namespace GardenHoseEngine.Frame;

public interface IUpdatedItem
{
    // Properties.
    public bool IsUpdated { get; set; }


    // Methods.
    public void Update(TimeSpan passedTime);

    public void OnDelete();
}