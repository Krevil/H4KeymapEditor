using System.IO;
using System.Windows;
using System.Buffers.Binary;

namespace H4KeymapEditor.Models
{
    public class Patcher
    {

        public static Dictionary<long, KeyBinding> OffsetMapping = KeyBinding.DefaultKeyBindings.ToDictionary(x => x.MemoryOffset);
        public const long SapienFileOffset = 0x746C3A;
        public const long TagTestFileOffset = 0x1FFE6A;
        public const long InstructionsLength = 0x350;

        public static void OpenFile(string filePath, ExecutableType exeType)
        {
            FileStream fs;
            try
            {
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            }
            catch (IOException)
            {
                MessageBox.Show("Could not open file");
                return;
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("No permission to access file");
                return;
            }

            if (!Validation.ValidateExecutable(fs, exeType))
            {
                MessageBox.Show("Could not validate executable");
                fs.Close();
                return;
            }
            byte[] bytes = new byte[InstructionsLength];
            if (exeType == ExecutableType.Sapien)
            {
                fs.Seek(SapienFileOffset, SeekOrigin.Begin);
            }
            else if (exeType == ExecutableType.TagTest)
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
            foreach (KeyBinding binding in newBindings)
            {
                KeyBinding.KeyBindings.Add(binding);
            }
        }

        public static void SaveFile(string filePath, ExecutableType exeType)
        {
            FileStream fs;
            try
            {
                fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                MessageBox.Show("Could not open file");
                return;
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("No permission to access file");
                return;
            }

            if (!Validation.ValidateExecutable(fs, exeType))
            {
                MessageBox.Show("Could not validate executable");
                fs.Close();
                return;
            }
            byte[] bytes = new byte[InstructionsLength];
            if (exeType == ExecutableType.Sapien)
            {
                fs.Seek(SapienFileOffset, SeekOrigin.Begin);
            }
            else if (exeType == ExecutableType.TagTest)
            {
                fs.Seek(TagTestFileOffset, SeekOrigin.Begin);
            }
            else
            {
                // This should be unreachable but just in case
                return;
            }
            fs.Read(bytes, 0, bytes.Length);
            WriteKeyBindings(bytes);
            if (exeType == ExecutableType.Sapien)
            {
                fs.Seek(SapienFileOffset, SeekOrigin.Begin);
            }
            else if (exeType == ExecutableType.TagTest)
            {
                fs.Seek(TagTestFileOffset, SeekOrigin.Begin);
            }
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
            MessageBox.Show("Saved file");
        }

        // Take section bytes from FileStream, read the keycodes and their offset 
        public static List<KeyBinding> ReadKeyBindings(byte[] bytes)
        {
            List<KeyBinding> bindings = new List<KeyBinding>();

            for (int i = 0; i < bytes.Length; i++)
            {
                long offset;
                // Instruction is mov
                int movOffset;
                Keycode keyCode;
                if (bytes[i] == 0xC7)
                {
                    if (bytes[i + 1] == 0x41) //8bit
                    {
                        offset = (sbyte)bytes[i + 2];
                        movOffset = i + 3;
                        keyCode = (Keycode)BitConverter.ToUInt32(bytes, movOffset);
                    }
                    else if (bytes[i + 1] == 0x81) //32bit
                    {
                        // Memory offset to compare against bindings dictionary
                        offset = BitConverter.ToInt32(bytes, i + 2);
                        // Offbyte in bytes to write to when saving new bindings back to file
                        movOffset = i + 6;
                        // Current keycode from the file
                        keyCode = (Keycode)BitConverter.ToUInt32(bytes, movOffset);
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

        // Get bytes from Sapien/tag_test, modify them using Keybindings
        public static void WriteKeyBindings(byte[] bytes)
        {
            foreach (KeyBinding binding in KeyBinding.KeyBindings)
            {
                if (!binding.MovOffset.HasValue)
                    continue;

                if (binding.MovOffset > bytes.Length)
                    continue;
                int offset = binding.MovOffset.Value;
                byte[] primaryKeyBytes = BitConverter.GetBytes((int)binding.PrimaryKey);
                Array.Copy(primaryKeyBytes, 0, bytes, offset, 4);
            }
        }
    }
}
