using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.IO.DataFile;

public abstract class DataFileReader : IDataFile
{
    // Fields.
    public abstract int FormatVersion { get; }


    // Methods.
    public abstract DataCompound? Read(string filePath, Func<int, bool> versionReadCallback);

    public bool ReadMetaData(BinaryReader reader, string filePath, Func<int, bool> versionReadCallback)
    {
        if (versionReadCallback == null)
        {
            throw new ArgumentNullException(nameof(versionReadCallback));
        }

        byte[] Signature = new byte[IDataFile.Signature.Length];
        if (reader.Read(Signature, 0, Signature.Length) < Signature.Length)
        {
            throw new DataReadException(filePath, $"File signature is shorter than expected.");
        }
        if (!Signature.SequenceEqual(IDataFile.Signature))
        {
            throw new DataReadException(filePath, $"Signature of data file is invalid.");
        }

        int FileFormatVersion = reader.ReadInt32();
        if (FileFormatVersion != FormatVersion)
        {
            throw new DataReadException(filePath,"File is saved in an unsupported format version." +
                $" (Expected {FormatVersion}, got {FileFormatVersion})");
        }

        return versionReadCallback.Invoke(reader.ReadInt32());
    }
}