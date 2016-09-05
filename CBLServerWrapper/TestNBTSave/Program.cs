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
            CommandChainEntry test = new CommandChainEntry()
            {
                Command = "setblock ~ ~1 ~ minecraft:dirt",
                Data = CommandBlockFlags.Repeat
            };

            for(int i = 1; i <= 40; i++)
            {
                CommandChainEntry last = test.EnumerateChain().Last();
                last.NextElement = new CommandChainEntry()
                {
                    Command = "say This is block #" + i,
                    Data = CommandBlockFlags.Chain | CommandBlockFlags.Conditional | CommandBlockFlags.AlwaysOn
                };
            }

            // Construct further test elements

            StructureFileCompiler.Instance.Save(test, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "cmdTest.nbt"));
            //NbtSaveUtilities.WriteCommandBlockToStructure(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "cmd.nbt"), "testfor @a", CommandBlockFlags.Impulse | CommandBlockFlags.AlwaysOn, CommandBlockDirection.Up);
        }
    }
}
