using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ex8.Helper.Globalization;
using Ex8.Helper.Serialization;
using Ude;

namespace Ex8.Helper.FileSystem
{
    public static class FileHelper
    {
        public static Encoding GetFileEncoding(string fullPath)
        {
            using (var fs = File.OpenRead(fullPath))
            {
                var charsetDetector = new CharsetDetector();
                charsetDetector.Feed(fs);
                charsetDetector.DataEnd();
                fs.Close();

                return EncodingHelper.GetEncodingFromCharSet(charsetDetector.Charset);
            }
        }

        public static IEnumerable<string> ReadLines(string fullPath)
        {
            var encoding = GetFileEncoding(fullPath);

            using (var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(fs, encoding, true))
            {
                string currentLine;
                while ((currentLine = reader.ReadLine()) != null)
                {
                    yield return currentLine;
                }
            }
        }

        public static string ReadText(string fullPath)
        {
            string content;
            using (var s = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var tr = new StreamReader(s))
            {
                content = tr.ReadToEnd();
            }

            return content;
        }

        public static T ReadText<T>(string fullPath)
        {
            var content = ReadText(fullPath);
            var returnObject = content.ParseJson<T>();

            return returnObject;
        }

        public static byte[] ReadBinary(FileInfo fileToCompress)
        {
            using (var originalFileStream = fileToCompress.OpenRead())
            {
                var b = new byte[originalFileStream.Length];
                originalFileStream.Read(b, 0, b.Length);

                originalFileStream.Close();

                return b;
            }
        }

        public static void WriteStreamToFile(Stream readStream, string filename)
        {
            var memStream = new MemoryStream();
            readStream.CopyTo(memStream);
            readStream.Dispose();

            WriteBytesToFile(memStream.ToArray(), filename);
        }

        public static void WriteTextLinesToFile(string fullPath, IEnumerable<string> lines, string headerRow = null)
        {
            DirectoryHelper.CreateIfNotExistDirectoryParent(fullPath);
            using (StreamWriter writer = File.CreateText(fullPath))
            {
                if (!string.IsNullOrEmpty(headerRow))
                    writer.WriteLine(headerRow);

                foreach (var line in lines)
                    writer.WriteLine(line);

                writer.Flush();
            }
        }

        public static void WriteTextToFile(string content, string fileFullPath)
        {
            DirectoryHelper.CreateIfNotExistDirectoryParent(fileFullPath);
            File.WriteAllText(fileFullPath, content, Encoding.UTF8);
        }

        public static void WriteBytesToFile(byte[] content, string fileFullPath)
        {
            DirectoryHelper.CreateIfNotExistDirectoryParent(fileFullPath);
            File.WriteAllBytes(fileFullPath, content);
        }
    }
}