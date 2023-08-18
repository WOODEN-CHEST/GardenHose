using System;


namespace GardenHoseEngine.Frame;

public interface IUpdateableItem
{
    // Properties.
    public bool IsUpdated { get; set; }


    // Methods.
    public void Update();

    public void Delete();
}