using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine;

public interface ITimeUpdatable
{
    public void Update(float passedTimeSeconds);
}