using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine;

public interface ITimeUpdatable
{
    public ITimeUpdater Updater { get; }

    public void Update(TimeSpan passedTime);

    public virtual void ForceRemove()
    {
        Updater.RemoveUpdateable(this);
    }
}