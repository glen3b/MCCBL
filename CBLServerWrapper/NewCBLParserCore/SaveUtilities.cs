using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewCBLParserCore
{
    public static class SaveUtilities
    {
        /// <summary>
        /// Writes a single command block to a structure file at the given path.
        /// </summary>
        /// <param name="filePath">The path to the location at which the NBT structure file will be saved.</param>
        /// <param name="command">The command to put inside the single command block in the structure.</param>
        /// <param name="properties">The properties of the single command block in the structure.</param>
        /// <param name="dir">The direction in which the command block will be facing.</param>
        public static void WriteCommandBlockToStructure(string filePath, string command, CommandBlockFlags properties, CommandBlockDirection dir)
        {
            NbtFile file = new NbtFile();
            
            file.RootTag = new NbtCompound("")
            {
                new NbtList("size")
                {
                    // X, Y, Z
                    new NbtInt(1),
                    new NbtInt(1),
                    new NbtInt(1)
                },
                //new NbtList("entities") { },
                new NbtList("palette")
                {
                    GetPaletteSignature(CommandBlockInfo.FromEnums(properties, dir))
                },
                new NbtList("blocks")
                {
                    new NbtCompound()
                    {
                        new NbtCompound("nbt")
                        {
                            new NbtByte("auto", Convert.ToByte(properties.HasFlag(CommandBlockFlags.AlwaysOn))),
                            new NbtString("Command",command),
                            new NbtString("id","Control"),
                            new NbtString("CustomName","@"),
                            new NbtByte("TrackOutput",0),
                            new NbtByte("powered",0),
                            new NbtByte("conditionMet",1),
                            new NbtInt("SuccessCount",0)
                        },
                        new NbtList("pos")
                        {
                            new NbtInt(0),
                            new NbtInt(0),
                            new NbtInt(0)
                        },
                        new NbtInt("state",0)

                    }
                },
                new NbtString("author", "MCCBL Compiler"),
                new NbtInt("version", 1)
            };
            file.SaveToFile(filePath, NbtCompression.GZip);

        }

        public static NbtCompound GetPaletteSignature(CommandBlockInfo info)
        {
            return new NbtCompound()
            {
                new NbtString("Name", info.CommandBlockID),
                new NbtCompound("Properties")
                {
                    new NbtString("conditional", info.IsConditional.ToString().ToLower()),
                    new NbtString("facing", info.FacingDirection.ToString().ToLower())
                }
            };
        }
    }
}
