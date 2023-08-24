using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GardenHoseEngine.Frame.UI.Animation;

public enum FinishLocation
{
    Start,
    End
}

public class AnimFinishEventArgs : EventArgs
{
    // Fields.
    public readonly FinishLocation FinishedAt;


    // Constructors.
    public AnimFinishEventArgs(FinishLocation finishLocation) => FinishedAt = finishLocation;
}