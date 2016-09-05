using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewCBLParserCore
{
    public struct CommandBlockInfo
    {
        public static CommandBlockInfo FromEnums(CommandBlockFlags flags, CommandBlockDirection dir)
        {
            return new CommandBlockInfo(flags, dir);
        }

        private CommandBlockInfo(CommandBlockFlags flags, CommandBlockDirection dir)
        {
            _facingDirection = dir;
            _flags = flags & ~(CommandBlockFlags.AlwaysOn | CommandBlockFlags.Chain | CommandBlockFlags.Impulse | CommandBlockFlags.Repeat);
            SetCommandBlockType(flags & (CommandBlockFlags.Chain | CommandBlockFlags.Impulse | CommandBlockFlags.Repeat));
        }

        public override int GetHashCode()
        {
            return _flags.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if(!(obj is CommandBlockInfo))
            {
                return false;
            }

            var cbi = (CommandBlockInfo)obj;
            return cbi._flags == _flags && cbi.FacingDirection == FacingDirection;
        }

        public static bool operator ==(CommandBlockInfo one, object other)
        {
            return one.Equals(other);
        }

        public static bool operator !=(CommandBlockInfo one, object other)
        {
            return !one.Equals(other);
        }

        private CommandBlockDirection _facingDirection;

        /// <summary>
        /// Gets the direction this command block is facing.
        /// </summary>
        public CommandBlockDirection FacingDirection
        {
            get { return _facingDirection; }
            set { _facingDirection = value; }
        }

        /// <summary>
        /// Gets the minecraft block identifier for this command block type.
        /// </summary>
        public string CommandBlockID
        {
            get
            {
                if (_flags.HasFlag(CommandBlockFlags.Repeat))
                {
                    return "minecraft:repeating_command_block";
                }
                if (_flags.HasFlag(CommandBlockFlags.Chain))
                {
                    return "minecraft:chain_command_block";
                }

                // Defaults to impulse
                return "minecraft:command_block";
            }
        }

        public void SetCommandBlockType(CommandBlockFlags type)
        {
            _flags &= ~(CommandBlockFlags.Impulse | CommandBlockFlags.Chain | CommandBlockFlags.Repeat);
            switch (type)
            {
                case CommandBlockFlags.Repeat:
                case CommandBlockFlags.Chain:
                case CommandBlockFlags.Impulse:
                    _flags |= type;
                    break;
                default:
                    throw new ArgumentException("Flags variables are not supported when setting the command block type.");
            }
        }

        // Do not set alwayson in this flags var
        private CommandBlockFlags _flags;

        /// <summary>
        /// Gets or sets a value indicating whether this command block is conditional.
        /// </summary>
        public bool IsConditional
        {
            get { return _flags.HasFlag(CommandBlockFlags.Conditional); }
            set
            {
                if (value)
                {
                    _flags |= CommandBlockFlags.Conditional;
                }
                else
                {
                    _flags &= ~CommandBlockFlags.Conditional;
                }
            }
        }

        public CommandBlockFlags GetDataFlags(bool alwaysOn = false)
        {
            // Make a copy
            CommandBlockFlags retFlags = _flags;
            if (alwaysOn)
            {
                retFlags |= CommandBlockFlags.AlwaysOn;
            }
            return retFlags;
        }

    }
}
