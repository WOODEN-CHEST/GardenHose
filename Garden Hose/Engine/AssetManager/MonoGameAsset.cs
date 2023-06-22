using System;


namespace GardenHose.Engine;

public struct MonoGameAsset<AssetType> where AssetType : class
{
    // Fields.
    public uint Users { get; private set; }

    // Private fields.
    private string _absolutePath = null;
    private AssetType _gameAsset = null;


    // Constructors.
    public MonoGameAsset(string path)
    {
        if (string.IsNullOrEmpty(path)) throw new ArgumentNullException($"Invalid asset path");
        _absolutePath = path;
        Users = 0;
    }


    // Methods.
    public AssetType GetAsset()
    {
        if (_gameAsset == null) LoadAsset();
        Users++;
        return _gameAsset;
    }

    public void DisposeUser()
    {
        if (_gameAsset == null) return;
        Users--;
    }

    public void LoadAsset()
    {
        _gameAsset = AssetManager.Content.Load<AssetType>(_absolutePath);
    }

    public void UnloadAsset()
    {
        AssetManager.Content.UnloadAsset(_absolutePath);
        _gameAsset = null;
    }
}
