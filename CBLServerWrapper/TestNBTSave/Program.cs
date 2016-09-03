using NewCBLParserCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestNBTSave
{
    class Program
    {
        static void Main(string[] args)
        {
            SaveUtilities.WriteCommandBlockToStructure(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "cmd.nbt"), "testfor @a", CommandBlockFlags.Impulse | CommandBlockFlags.AlwaysOn, CommandBlockDirection.Up);
        }
    }
}
