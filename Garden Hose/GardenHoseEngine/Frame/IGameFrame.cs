using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework.Graphics;

namespace GardenHoseEngine.Frame;

public interface IGameFrame : IColorMaskable
{
    // Properties.
    public string Name { get; }

    public bool IsLoaded { get; }

    public ILayer? TopLayer { get; }

    public ILayer? BottomLayer { get; }

    public int LayerCount { get; }

    public PropertyAnimManager AnimationManager { get; }


    // Methods.
    /* Layers. */
    public void AddLayer(ILayer layer);

    public void RemoveLayer(ILayer layer);

    public ILayer? GetLayer(int index);

    public ILayer? GetLayer(string name);


    /* Items. */
    public void AddItem(ITimeUpdatable item);

    public void RemoveItem(ITimeUpdatable item);

    public void ClearItems();


    /* Status control. */
    public void Load();

    public void OnStart();

    public void Restart();

    public void OnEnd();

    public void Unload();


    /* Updates and draws. */
    public void Draw(IDrawInfo info, RenderTarget2D layerPixelBuffer, RenderTarget2D framePixelBuffer);

    public void Update(IProgramTime time);
}