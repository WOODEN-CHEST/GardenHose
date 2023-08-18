using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace GardenHoseEngine.Frame;

public struct GameLoopInfo
{
    // Fields.
    public int TotalDraws => TextureDraws + StringDraws;
    public int TextureDraws;
    public int StringDraws;
    public int DrawBatchCount;

    public TimeSpan DrawTime;
    public TimeSpan UpdateTime;


    // Constructors.
    public GameLoopInfo()
    {
        TextureDraws = 0;
        StringDraws = 0;
        DrawBatchCount = 0;
    }


    // Methods.
    public void ResetDrawCount()
    {
        TextureDraws = 0;
        StringDraws = 0;
        DrawBatchCount = 0;
    }
}