using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace GardenHoseEngine;

public class Test
{
    // Static fields.
    public const string DIR_TEXTURES = "textures";
    public const string DIR_SOUNDS = "sounds";
    public const string DIR_FONTS = "fonts";
    public const string DIR_SHADERS = "shaders";


    // Fields.
    public readonly string BasePath;
    public readonly string? ExtraPath;
    public readonly HashSet<string> NonOverridableDirectories = new();


    // Private fields.
    private readonly Dictionary<string, (string rootPath, uint userCount)> _assets = new();
    private readonly ContentManager _content;
    private readonly GHEngine _ghEngine;


    // Constructors.
    public Test(string basePath, string? extraPath, GHEngine engine, Game game)
    {
        _ghEngine = engine;
        _content = game.Content;

        BasePath = basePath ?? throw new ArgumentNullException(nameof(basePath));
        if (!Path.IsPathFullyQualified(basePath))
        {
            throw new ArgumentException($"Base path \"{basePath}\" is not fully qualified");
        }

        ExtraPath = extraPath;
        if ((extraPath != null) && (!Path.IsPathFullyQualified(extraPath)))
        {
            throw new ArgumentException($"Extra path \"{extraPath}\" is not fully qualified");
        }

        ScanBaseAssets();
    }


    // Methods.
    public void ScanExtraAssets(string folderName)
    {
        if (folderName == null)
        {
            throw new ArgumentNullException(nameof(folderName));
        }
        if (ExtraPath == null)
        {
            throw new InvalidOperationException("Cannot scan extra path since it's null");
        }

        string AssetRootPath = Path.Combine(ExtraPath, folderName);
        if (!Directory.Exists(AssetRootPath))
        {
            throw new ArgumentException($"Folder \"{folderName}\" not found.");
        }

        WalkDirectory(AssetRootPath);

        void WalkDirectory(string directory)
        {
            if (NonOverridableDirectories.Contains(directory))
            {
                GH
                return;
            }

            foreach (string FullPath in Directory.GetFiles(directory))
            {
                string RelativePath = Path.GetRelativePath(AssetRootPath, FullPath);

                if (!_assets.TryGetValue(RelativePath, out var Asset)) continue;

                Asset.fullPath = FullPath;
                _assets[RelativePath] = Asset;
            }

            foreach (string SubDirectory in Directory.GetDirectories(directory))
            {
                WalkDirectory(SubDirectory);
            }
        }
    }

    public void UseAssets(IEnumerable<string> assetRelativePaths)
    {
        foreach (string relativePath in assetRelativePaths)
        {
            if (!_assets.TryGetValue(relativePath, out var Asset))
            {
                ThrowAssetNotFound(relativePath);
            }

            Asset.userCount++;
            _assets[relativePath] = Asset;
        }
    }

    public void ReleaseAssets(IEnumerable<string> relativePaths)
    {
        foreach (string relativePath in relativePaths)
        {
            if (!_assets.TryGetValue(relativePath, out var Asset))
            {
                ThrowAssetNotFound(relativePath);
            }

            try
            {
                checked { Asset.userCount--; }
            }
            catch (OverflowException e)
            {
                throw new ArgumentException
                    ($"Couldn't release asset \"{relativePath}\" since it already had 0 users. {e}");
            }

            _assets[relativePath] = Asset;
        }
    }

    public void LoadAssets(IEnumerable<string> relativePaths)
    {
        foreach (string relativePath in relativePaths)
        {
            if (!_assets.TryGetValue(relativePath, out var Asset))
            {
                ThrowAssetNotFound(relativePath);
            }

            if (Asset.userCount == 0)
            {
                throw new InvalidOperationException("Cannot load an asset since it has 0 users.");
            }

            _content.Load<object>(relativePath);
        }
    }

    public TAsset GetAsset<TAsset>(string relativePath) where TAsset : class
    {
        if (!_assets.TryGetValue(relativePath, out var Asset))
        {
            ThrowAssetNotFound(relativePath);
        }

        if (Asset.userCount == 0u)
        {
            throw new InvalidOperationException("Asset cannot be retrieved since it has 0 users");
        }

        return _content.Load<TAsset>(Asset.fullPath);
    }


    // Internal methods.
    internal void UnloadUnneededAssets()
    {
        foreach ((string fullpath, uint userCount) in _assets.Values)
        {
            if (userCount == 0)
            {
                _content.UnloadAsset(fullpath);
            }
        }
    }


    // Private methods.
    private void ScanBaseAssets()
    {
        if (_assets.Count != 0)
        {
            throw new InvalidOperationException("Base assets have already been scanned");
        }

        WalkDirectory(BasePath);

        void WalkDirectory(string directory)
        {
            foreach (string File in Directory.GetFiles(directory))
            {
                _assets.Add(Path.GetRelativePath(BasePath, File), (File, 0u));
            }

            foreach (string SubDirectory in Directory.GetDirectories(directory))
            {
                WalkDirectory(SubDirectory);
            }
        }

        if (_assets.Count == 0)
        {
            throw new Exception($"Found no assets in the base path \"{BasePath}\"");
        }
    }

    private void LoadAssetIntoMemory(string relativePath)
    {
        if (relativePath == null)
        {
            throw new ArgumentNullException(nameof(relativePath));
        }


    }

    private void ThrowAssetNotFound(string assetPath)
    {
        throw new ArgumentException($"Asset \"{assetPath}\" not found.");
    }
}