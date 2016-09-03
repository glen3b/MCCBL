using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewCBLParserCore
{
    /// <summary>
    /// Represents data about the type of a command block.
    /// Defaults to a non-conditional impulse command block that is not always on.
    /// </summary>
    [Flags]
    public enum CommandBlockFlags : ushort
    {
        /// <summary>
        /// Represents an impulse command block type.
        /// </summary>
        Impulse = 0x0,
        /// <summary>
        /// Represents a command block that is part of a chain.
        /// </summary>
        Chain = 0x01,
        /// <summary>
        /// Represents a repeating command block.
        /// </summary>
        Repeat = 0x02,
        /// <summary>
        /// Represents a command block that will only trigger if the one behind it geometrically (not the triggering command block in a chain) successfully ran its command.
        /// </summary>
        Conditional = 0x04,
        /// <summary>
        /// Represents a command block that is always on.
        /// </summary>
        AlwaysOn = 0x08
    }
}
