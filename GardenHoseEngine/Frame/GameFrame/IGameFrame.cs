using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.Frame;

public interface IGameFrame
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
    public void AddItem(ITimeUpdatable item);

    public void RemoveItem(ITimeUpdatable item);

    public void ClearItems();


    /* Status control. */
    public void BeginLoad();

    public void Load();

    public void FinalizeLoad();

    public void OnStart();

    public void Restart();

    public void OnEnd();

    public void Unload();


    /* Updates and draws. */
    public void Draw();

    public void Update();
}