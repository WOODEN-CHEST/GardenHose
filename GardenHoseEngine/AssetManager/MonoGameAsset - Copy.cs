﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace GardenHoseEngine;

public struct MonoGameAsset<AssetType> where AssetType : class
{
    // Fields.
    public uint Users { get; private set; }

    // Private fields.
    private readonly string _absolutePath;
    private AssetType? _gameAsset;


    // Constructors.
    public MonoGameAsset(string path)
    {

        if (string.IsNullOrEmpty(path)) throw new ArgumentNullException();
        _absolutePath = path ?? throw new ArgumentNullException(nameof(path));
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
        _gameAsset = gfdgdfg.Content.Load<AssetType>(_absolutePath);
    }

    public void UnloadAsset()
    {
        gfdgdfg.Content.UnloadAsset(_absolutePath);
        _gameAsset = null;
    }
}
