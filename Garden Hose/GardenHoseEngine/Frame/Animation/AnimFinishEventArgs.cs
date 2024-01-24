namespace GardenHoseEngine.Frame.Animation;

public class AnimFinishEventArgs : EventArgs
{
    // Fields.
    public readonly FinishLocation FinishedLocation;


    // Constructors.
    public AnimFinishEventArgs(FinishLocation finishLocation) => FinishedLocation = finishLocation;
}