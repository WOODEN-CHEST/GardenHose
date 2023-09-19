using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.IO.DataFile;

public abstract class DataFileWriter : IDataFile
{
    // Fields.
    public abstract int FormatVersion { get; }

    public IWriteableDataCompound BaseCompound { get; }


    // Constructors.
    public DataFileWriter(IWriteableDataCompound baseCompound)
    {
        if (baseCompound == null)
        {
            throw new ArgumentNullException(nameof(baseCompound));
        }
        if (baseCompound.FormatVersion != FormatVersion)
        {
            throw new ArgumentException(
                $"Unsupported base compound version: {baseCompound.FormatVersion}, expected {FormatVersion}.",
                nameof(baseCompound));
        }

        BaseCompound = baseCompound;
    }


    // Methods.
    public abstract void Write(string path, int gameVersion, IWriteableDataCompound dataCompound);


    // Internal methods.
    internal void WriteMetaData(BinaryWriter writer, int gameVersion)
    {
        if (writer == null)
        {
            throw new InvalidOperationException("Stream writer is not set.");
        }

        writer.Write(IDataFile.Signature);
        writer.Write(FormatVersion);
        writer.Write(gameVersion);
    }
}