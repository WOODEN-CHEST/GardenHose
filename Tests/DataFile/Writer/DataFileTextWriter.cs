using NAudio.SoundFont;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.IO.DataFile;

public abstract class DataFileTextWriter : IDataFile
{
    // Fields.
    public const int LatestVersion = 1;

    public abstract int FormatVersion { get; }


    // Static methods.
    public static DataFileTextWriter GetWriter(int version = LatestVersion)
    {
        return version switch
        {
            1 => new DataFileTextWriterV1(),

            _ => throw new ArgumentOutOfRangeException(nameof(version),
                $"The version {version} of {nameof(DataFileTextWriter)} is not supported."),
        };
    }


    // Methods.
    public void Write(string path, DataFileCompound compound)
    {

    }
}