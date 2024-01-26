using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.IO;

public interface IInputListener
{
    public void Listen(bool windowFocused);

    public void StopListening();

    public void StartListening();
}