using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GardenHose.Engine.IO.DataFile;

public class DataReadException : Exception
{
    // Constructors.
    public DataReadException() : base("Failed to read file data due to an unknown reason.") { }
    public DataReadException(string message) : base(message) { }
}