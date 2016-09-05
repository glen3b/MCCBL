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
        static bool AreCompoundsEqual(NbtCompound left, NbtCompound right)
        {
            if (left.Count != right.Count)
            {
                return false;
            }

            if (left.Name != right.Name)
            {
                // The tag names themselves
                return false;
            }

            // Use default ordering: ensure we have a consistent order across both lists so we can compare more easily
            string[] leftNames = left.Names.OrderBy(e => e).ToArray();
            string[] rightNames = right.Names.OrderBy(e => e).ToArray();

            for (int i = 0; i < leftNames.Length; i++)
            {
                if (!string.Equals(leftNames[i], rightNames[i], StringComparison.Ordinal))
                {
                    return false;
                }
            }

            // All names are equal, now compare values
            // Names are equal so which list we loop doesn't matter
            // Loop by name not by index so order isn't an issue
            foreach (string name in rightNames)
            {
                NbtTag leftVal = left[name], rightVal = right[name];
                if (leftVal.TagType != rightVal.TagType)
                {
                    return false;
                }

                switch (leftVal.TagType)
                {
                    case NbtTagType.Compound:
                        if(!AreCompoundsEqual(leftVal as NbtCompound, rightVal as NbtCompound))
                        {
                            return false;
                        }
                        break;
                    case NbtTagType.String:
                        if (!string.Equals(leftVal.StringValue, rightVal.StringValue, StringComparison.Ordinal))
                        {
                            return false;
                        }
                        break;
                    case NbtTagType.ByteArray:
                        if(!Enumerable.SequenceEqual(leftVal.ByteArrayValue, rightVal.ByteArrayValue))
                        {
                            return false;
                        }
                        break;
                    case NbtTagType.IntArray:
                        if (!Enumerable.SequenceEqual(leftVal.IntArrayValue, rightVal.IntArrayValue))
                        {
                            return false;
                        }
                        break;
                    default:
                        // No silent unexpected results
                        throw new NotImplementedException("The tag type " + leftVal.TagType + " is not supported by the compound comparison.");
                }
            }

            // All checks passed
            return true;
        }

        /// <summary>
        /// Saves a command block chain to a structure file at the given path.
        /// </summary>
        /// <param name="chainStart">The beginning of the command block chain.</param>
        public static void Save(CommandChainEntry chainStart, string savePath)
        {
            CommandChainEntry[] chain = chainStart.EnumerateChain().ToArray();
            CommandBlockDirection dir = CommandBlockDirection.East;

            int rowFlat = 0;
            int height = 0;
            int columnFlat = 0;

            NbtFile file = new NbtFile();

            file.RootTag = new NbtCompound("")
            {
                new NbtList("size")
                {
                    // TODO calculate size dynamically
                    new NbtInt(32),
                    new NbtInt(32),
                    new NbtInt(32)
                },
                //new NbtList("entities") { },
                new NbtList("palette")
                {
                },
                new NbtList("blocks")
                {
                },
                new NbtString("author", "MCCBL Compiler"),
                new NbtInt("version", 1)
            };

            for (int i = 0; i < chain.Length; i++)
            {
                int state = -1;
                rowFlat = i / 32;
                columnFlat = i % 32;
                dir = rowFlat % 2 == 0 ? CommandBlockDirection.East : CommandBlockDirection.West;
                if (columnFlat == 31)
                {
                    dir = height % 2 == 0 ? CommandBlockDirection.South : CommandBlockDirection.North;
                }

                //foreach (NbtCompound paletteE in ((NbtList)file.RootTag.Get("palette")))
                //{
                //    if (paletteE == GetPaletteSignature(CommandBlockInfo.FromEnums(chain[i].Data, dir)))
                //    {
                //        alreadyInPalette = true;
                //        break;
                //    }
                //    state++;
                //}

                NbtCompound palSig = GetPaletteSignature(CommandBlockInfo.FromEnums(chain[i].Data, dir));
                var paletteList = file.RootTag.Get<NbtList>("palette");

                // TODO does NbtCompound implement value equality? If not, we may have to use the general purpose IndexOf with our own comparison interface
                // Tested and does not

                // state = paletteList.IndexOf(palSig);

                for (int j = 0; j < paletteList.Count; j++)
                {
                    NbtCompound curPal = paletteList[j] as NbtCompound;
                    if (curPal == null)
                    {
                        continue;
                    }

                    if (AreCompoundsEqual(curPal, palSig))
                    {
                        // We found an equivalent palette entry
                        state = j;
                        break;
                    }
                }

                if (state == -1)
                {
                    paletteList.Add(palSig);
                    // TODO is this needed?
                    // file.RootTag["palette"] = paletteList;
                    state = paletteList.Count - 1;
                }
                NbtCompound block = new NbtCompound();
                if (rowFlat % 2 == 0)
                {
                    block.Add(new NbtList("pos") { new NbtInt(columnFlat), new NbtInt(height), new NbtInt(rowFlat) });
                }
                else
                {
                    block.Add(new NbtList("pos") { new NbtInt(31 - columnFlat), new NbtInt(height), new NbtInt(rowFlat) });
                }
                    block.Add(new NbtCompound("nbt")
                        {
                            new NbtByte("auto", Convert.ToByte(chain[i].Data.HasFlag(CommandBlockFlags.AlwaysOn))),
                            new NbtString("Command",chain[i].Command),
                            new NbtString("id","Control"),
                            new NbtString("CustomName","@"),
                            new NbtByte("TrackOutput",0),
                            new NbtByte("powered",0),
                            new NbtByte("conditionMet",1),
                            new NbtInt("SuccessCount",0)
                        });


                    block.Add(new NbtInt("state", state));
                    file.RootTag.Get<NbtList>("blocks").Add(block);

                }
                file.SaveToFile(savePath, NbtCompression.GZip);

            }
        


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
                    new NbtInt(32),
                    new NbtInt(32),
                    new NbtInt(32)
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
