using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.Frame;

public interface IGameFrame : ITimeUpdater
{
    // Properties.
    public string Name { get; }

    public bool IsLoaded { get; }

    public ILayer? TopLayer { get; }

    public ILayer? BottomLayer { get; }

    public int LayerCount { get; }


    // Methods.
    /* Layers. */
    public void AddLayer(ILayer layer);

    public void RemoveLayer(ILayer layer);

    public ILayer? GetLayer(int index);

    public ILayer? GetLayer(string name);


    /* Items. */
    public void ClearUpdateables();


    /* Status control. */
    public void Load(AssetManager assetManager);

    public void OnStart();

    public void Restart();

    public void OnEnd();

    public void Unload(AssetManager assetManager);


    /* Updates and draws. */
    public void Draw(TimeSpan passedTime, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, RenderTarget2D layerPixelBuffer);

    public void Update(TimeSpan passedTime);
}