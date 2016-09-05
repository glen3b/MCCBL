using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewCBLParserCore
{
    public class CommandChainEntry
    {
        public IEnumerable<CommandChainEntry> EnumerateChain()
        {
            CommandChainEntry retVal = this;
            do
            {
                yield return retVal;
                retVal = retVal.NextElement;
            } while (retVal != null);
        }

        private CommandChainEntry _next;

        public CommandChainEntry NextElement
        {
            get { return _next; }
            set
            {
                _next = value;
            }
        }

        public CommandBlockFlags Data { get; set; }
        public string Command
        {
            get
            {
                return _cmd;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(Command));
                }
                _cmd = value;
            }
        }
        private string _cmd;
    }
}
