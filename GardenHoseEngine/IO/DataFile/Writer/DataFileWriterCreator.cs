using NAudio.SoundFont;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.IO.DataFile;

public static class DataFileWriterCreator
{
    // Fields.
    public const int LatestVersion = 1;


    // Static methods.
    public static DataFileWriter GetWriter(int version = LatestVersion)
    {
        return version switch
        {
            1 => new DataFileWriterVersion1(),

            _ => throw new ArgumentOutOfRangeException(nameof(version),
                $"The version {version} of {nameof(DataFileWriter)} is not supported."),
        };
    }

    public static IWriteableDataCompound GetCompound(int? id, int version = LatestVersion)
    {
        return version switch
        {
            1 => new WriteableDataCompoundVersion1(1),

            _ => throw new ArgumentOutOfRangeException(nameof(version),
                $"The version {version} of {nameof(IWriteableDataCompound)} is not supported."),
        };
    }
}