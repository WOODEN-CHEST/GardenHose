using GardenHoseEngine.Assets;
using GardenHoseEngine.Audio;
using GardenHoseEngine.Frame;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
namespace GardenHoseEngine;

internal class GameAsset
{
    // Internal fields.
    internal bool IsUsed => _users.Count != 0;
    internal object? Asset { get; private set; } = null;
    internal string RelativePath { get; private set; }
    internal AssetType Type { get; private init; }
    internal bool IsPerpertual { get; set; } = false;


    // Private fields.
    private readonly List<IGameFrame> _users = new(2);


    // Constructors.
    internal GameAsset(string relativePath, AssetType assetType)
    {
        RelativePath = relativePath ?? throw new ArgumentNullException(nameof(assetType));
        Type = assetType;
    }


    // Internal methods.
    internal void AddUser(IGameFrame? user, ContentManager contentManager)
    {
        if (user == null)
        {
            IsPerpertual = true;
        }
        else
        {
            if (!_users.Contains(user))
            {
                _users.Add(user);
            }
        }
        
        LoadAsset(contentManager);
    }

    internal void RemoveUser(IGameFrame user, ContentManager contentManager)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        _users.Remove(user);
        TryFreeMemory(contentManager);
    }

    // Private methods.
    private void TryFreeMemory(ContentManager contentManager)
    {
        if (IsUsed || IsPerpertual) return;

        switch (Type)
        {
            case AssetType.Texture:
            case AssetType.Font:
            case AssetType.Shader:
                contentManager.UnloadAsset(RelativePath);
                break;

            case AssetType.Sound:
                break; // Simply removing the reference is enough to unload sounds.

            default:
                throw new EnumValueException(nameof(Type), Type);
        }

        Asset = null;
    }

    private void LoadAsset(ContentManager contentManager)
    {
        if (Asset != null)
        {
            return;
        }

        Asset = Type switch
        {
            AssetType.Texture => contentManager.Load<Texture2D>(RelativePath),
            AssetType.Sound => new Sound(RelativePath, AudioEngine.Engine);
            AssetType.Font => contentManager.Load<SpriteFont>(RelativePath),
            AssetType.Shader => contentManager.Load<Effect>(RelativePath),
            _ => throw new EnumValueException(nameof(Type), Type)
        };
    }
}