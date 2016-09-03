using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewCBLParserCore
{
    /// <summary>
    /// Represents a command block's direction.
    /// </summary>
    public enum CommandBlockDirection : byte
    {
        North,
        South,
        East,
        West,
        Up,
        Down
    }
}
