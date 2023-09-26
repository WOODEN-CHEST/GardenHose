using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.Frame;

public interface IGameFrameManager
{
    // Properties.
    public IGameFrame ActiveFrame { get; }
    public IGameFrame? NextFrame { get; }


    // Methods.
    public void UpdateFrames(float passedTimeSeconds);

    public void DrawFrames(float passedTimeSeconds);

    public void LoadNextFrame(IGameFrame nextFrame, Action onLoadComplete);
    public void JumpToNextFrame();
}