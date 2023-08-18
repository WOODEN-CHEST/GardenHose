using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GardenHose.Engine.IO.DataFile;

public class DataWriteException : Exception
{
    // Constructors.
    public DataWriteException() : base("Failed to write data file due to an unknown reason.") { }
    public DataWriteException(string message) : base(message) { }
}