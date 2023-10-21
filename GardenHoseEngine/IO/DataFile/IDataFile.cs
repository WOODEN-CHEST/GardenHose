using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.IO.DataFile;

public interface IDataFile
{
    // Fields.
    public const string FileExtension = ".ghdf";

    public static readonly byte[] Signature =
    { 102, 37, 143, 181, 3, 205, 123, 185, 148, 157, 98, 177, 178, 151, 43, 170 };

    public int FormatVersion { get; }

    public const int TERMINATING_ID = 0;
}