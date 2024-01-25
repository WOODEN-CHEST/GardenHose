namespace GardenHoseEngine.Frame;

public class AnimFinishEventArgs : EventArgs
{
    // Fields.
    public readonly FinishLocation FinishedLocation;


    // Constructors.
    public AnimFinishEventArgs(FinishLocation finishLocation) => FinishedLocation = finishLocation;
}