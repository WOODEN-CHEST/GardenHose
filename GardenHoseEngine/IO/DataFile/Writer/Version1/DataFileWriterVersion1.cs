using GardenHoseEngine.IO.DataFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace GardenHoseEngine.IO.DataFile;

public sealed class DataFileWriterVersion1 : DataFileWriter
{
    // Public fields.
    public override int FormatVersion { get; } = 1;


    // Constructors.
    internal DataFileWriterVersion1() 
        : base(new WriteableDataCompoundVersion1()) { }

    // Methods.
    public override void Write(string path, int gameVersion, IWriteableDataCompound dataCompound)  
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }
        if (dataCompound == null)
        {
            throw new ArgumentNullException(nameof(dataCompound));
        }
        if (dataCompound.FormatVersion != 1)
        {
            throw new ArgumentException($"Unsupported data compound version: {dataCompound.FormatVersion}");
        }

        path = Path.ChangeExtension(path, IDataFile.FileExtension);

        try
        {
            string? DirectoryPath = Path.GetDirectoryName(path);
            if (DirectoryPath == null)
            {
                throw new ArgumentException($"Invalid path \"{DirectoryPath}\". (Directory is a root directory)", 
                    nameof(DirectoryPath));
            }

            Directory.CreateDirectory(DirectoryPath);

            using BinaryWriter Writer = new(File.Create(path));
            WriteMetaData(Writer, gameVersion);
            Writer.Write(dataCompound.Data);
        }
        catch (IOException e)
        {
            throw new DataWriteException(path, $"IOException writing DataFile. {e}");
        }
        catch (UnauthorizedAccessException e)
        {
            throw new DataWriteException(path, $"Denied permission to write file. {e}");
        }
    }
}