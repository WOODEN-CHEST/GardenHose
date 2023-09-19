using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.IO.DataFile;

public static class DataFileReaderCreator
{
    // Fields.
    public const int LatestVersion = 1;


    // Static methods.
    public static DataFileReader GetReader(int version = LatestVersion)
    {
        return version switch
        {
            1 => new DataFileReaderVersion1(),

            _ => throw new ArgumentOutOfRangeException(nameof(version),
                $"The version {version} of {nameof(DataFileReader)} is not supported."),
        };
    }
}