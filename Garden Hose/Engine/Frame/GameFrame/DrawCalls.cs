using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace GardenHose.Engine.Frame;

public struct GameLoopInfo
{
    // Fields.
    public int TotalDraws { get => TextureDraws + StringDraws + BasicDraws; }
    public int TextureDraws;
    public int StringDraws;
    public int BasicDraws;
    public int DrawBatchCount;

    public TimeSpan DrawTime;
    public TimeSpan UpdateTime;


    // Constructors.
    public GameLoopInfo()
    {
        TextureDraws = 0;
        StringDraws = 0;
        BasicDraws = 0;
        DrawBatchCount = 0;
    }


    // Methods.
    public void ResetDrawCount()
    {
        TextureDraws = 0;
        StringDraws = 0;
        BasicDraws = 0;
        DrawBatchCount = 0;
    }
}