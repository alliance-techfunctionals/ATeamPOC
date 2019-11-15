using System;
using System.Text;

namespace Ex8.Helper.Globalization
{
    public static class EncodingHelper
    {
        public static Encoding GetEncodingFromCharSet(string charset)
        {
            if (charset == null)
            {
                throw new Exception($"Unsupported file encoding type. Charset:{charset}");
            }

            switch (charset)
            {
                case "ASCII":
                    return Encoding.ASCII;
                case "UTF-8":
                    return Encoding.UTF8;
                case "UTF-16LE":
                    return Encoding.Unicode;
                case "UTF-16BE":
                    return Encoding.BigEndianUnicode;
                case "UTF-32BE":
                    return new UTF32Encoding(true, true);
                case "UTF-32LE":
                    return Encoding.UTF32;
                case "windows-1252":
                    return Encoding.GetEncoding(1252);
                default:
                    throw new Exception($"Unsupported file encoding type. Charset:{charset}");
            }
        }
    }
}