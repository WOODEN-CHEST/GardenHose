﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine;

public interface ITimeUpdater
{
    public void AddUpdateable(ITimeUpdatable item);

    public void RemoveUpdateable(ITimeUpdatable item);
}