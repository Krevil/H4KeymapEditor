using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace H4KeymapEditor.Models
{
    public enum ExecutableType : int
    {
        Sapien,
        SapienPlay,
        TagTest,
        TagPlay
    }

    public class Validation
    {
        public static readonly long SapienOffset = 0x746C32;
        public static readonly long TagTestOffset = 0x1FFE62;
        public static readonly byte[] ValidationBytes = { 0x48, 0x83, 0xEA, 0x01, 0x75, 0xD8, 0x33, 0xC0 };

        public static bool ValidateExecutable(FileStream fs, ExecutableType exeType)
        {
            long offset = -1;
            switch (exeType)
            {
                case ExecutableType.Sapien:
                    offset = SapienOffset;
                    break;
                case ExecutableType.SapienPlay:
                    break;
                case ExecutableType.TagTest:
                    offset = TagTestOffset;
                    break;
                case ExecutableType.TagPlay:
                    break;
            }

            if (offset == -1) return false;

            if (offset > fs.Length) return false;

            fs.Seek(offset, SeekOrigin.Begin);
            byte[] bytes = new byte[ValidationBytes.Length];
            fs.Read(bytes, 0, bytes.Length);

            return bytes.SequenceEqual(ValidationBytes);
        }
    }
}
