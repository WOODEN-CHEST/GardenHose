using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.Screen;

public interface IVirtualConverter
{
    // Properties.
    public float ItemScale { get; }

    public float InverseItemScale { get; }

    public Vector2 ItemOffset { get; }


    // Methods.
    public Vector2 ToRealPosition(Vector2 virtualVector);

    public Vector2 ToVirtualPosition(Vector2 realVector);

    public Vector2 ToRealScale(Vector2 virtualScale);

    public Vector2 ToVirtualScale(Vector2 realScale);
}