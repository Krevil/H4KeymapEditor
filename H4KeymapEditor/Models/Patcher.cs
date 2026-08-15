using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Buffers.Binary;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Security.AccessControl;
using System.Windows.Data;

namespace H4KeymapEditor.Models
{
    public class Patcher
    {

        public static Dictionary<long, KeyBinding> OffsetMapping = KeyBinding.DefaultKeyBindings.ToDictionary(x => x.MemoryOffset);
        public const long SapienFileOffset = 0x746C3A;
        public const long TagTestFileOffset = 0x1FFE6A;
        public const long InstructionsLength = 0x350;

        public static void OpenFile(string filePath)
        {
            ExecutableType newExeType;
            if (filePath.Contains("sapien"))
            {
                if (filePath.Contains("sapien_play"))
                {
                    MessageBox.Show("Play executables not currently supported");
                    return;
                }
                newExeType = ExecutableType.Sapien;
            }
            else if (filePath.Contains("tag_test"))
            {
                newExeType = ExecutableType.TagTest;
            }
            else if (filePath.Contains("tag_play"))
            {
                MessageBox.Show("Play executables not currently supported");
                return;
            }
            else
            {
                MessageBox.Show("Executable must be either sapien or tag_test");
                return;
            }

            // If there is current keybindings loaded
            if (KeyBinding.KeyBindings.Count > 0)
            {
                // handle save and close
                var result = MessageBox.Show("Save current keybindings?", "Save keybindings?", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    SaveFile();
                }
            }

            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            if (!Validation.ValidateExecutable(fs, newExeType))
            {
                MessageBox.Show("Could not validate executable");
                fs.Close();
                return;
            }
            byte[] bytes = new byte[InstructionsLength];
            if (newExeType == ExecutableType.Sapien)
            {
                fs.Seek(SapienFileOffset, SeekOrigin.Begin);
            }
            else if (newExeType == ExecutableType.TagTest)
            {
                fs.Seek(TagTestFileOffset, SeekOrigin.Begin);
            }
            else
            {
                // This should be unreachable but just in case
                return;
            }
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            List<KeyBinding> newBindings = ReadKeyBindings(bytes);
            KeyBinding.KeyBindings.Clear();
            foreach (KeyBinding binding in  newBindings)
            {
                KeyBinding.KeyBindings.Add(binding);
            }
        }

        public static void SaveFile()
        {

        }

        // Take section bytes from FileStream, read the keycodes and their offset 
        public static List<KeyBinding> ReadKeyBindings(byte[] bytes)
        {
            List<KeyBinding> bindings = new List<KeyBinding>();

            for (int i = 0; i < bytes.Length; i++)
            {
                long offset;
                // Instruction is mov
                uint movOffset;
                Keycode keyCode;
                if (bytes[i] == 0xC7)
                {
                    if (bytes[i + 1] == 0x41) //8bit
                    {
                        offset = (sbyte)bytes[i + 2];
                        movOffset = (uint)(i + 6);
                        keyCode = (Keycode)BitConverter.ToUInt32(bytes, i + 3);
                    }
                    else if (bytes[i + 1] == 0x81) //32bit
                    {
                        // Memory offset to compare against bindings dictionary
                        offset = BitConverter.ToInt32(bytes, i + 2);
                        // Offbyte in bytes to write to when saving new bindings back to file
                        movOffset = (uint)(i + 6);
                        // Current keycode from the file
                        keyCode = (Keycode)BitConverter.ToUInt32(bytes, i + 6);
                    }
                    else
                        continue;
                }
                else
                    continue;

                    offset -= KeyBinding.FileOffsetBase;
                // Only looking for offsets that start at 5D0 or above
                if (offset < 0) continue;
                if (OffsetMapping.TryGetValue(offset, out KeyBinding keyBinding))
                {
                    keyBinding.MovOffset = movOffset;
                    keyBinding.PrimaryKey = keyCode;
                    bindings.Add(keyBinding);
                }
            }
            return bindings;
        }
    }
}
