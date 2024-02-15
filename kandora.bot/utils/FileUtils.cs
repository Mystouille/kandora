using System.IO;
using System.Linq;
using System.Reflection;

namespace kandora.bot.utils
{
    public class FileUtils
    {
        private static string resourcesDirName = "resources";
        private static string outputDirName = "gameLogs";
        private static string fileExtension = ".json";
        private static char dirChar = Path.DirectorySeparatorChar;

        public static string inputResourceLoc = "kandora.bot.resources";

        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new();
            using StreamWriter writer = new(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static Stream GetStreamFromResourceFile(string fileName)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream($"{inputResourceLoc}.{fileName}");
        }

        public static bool IsLogCached(string logName)
        {
            string filePath = string.Join(dirChar, Assembly.GetExecutingAssembly().Location.Split(dirChar).SkipLast(1).Append(resourcesDirName).Append(outputDirName).Append(logName + fileExtension));
            return File.Exists(filePath);
        }
        public static string ReadLog(string logName)
        {
            string filePath = string.Join(dirChar, Assembly.GetExecutingAssembly().Location.Split(dirChar).SkipLast(1).Append(resourcesDirName).Append(outputDirName).Append(logName + fileExtension));
            return File.ReadAllText(filePath);
        }
        public static void SaveLog(string logName, string content)
        {
            string dirPath = string.Join(dirChar, Assembly.GetExecutingAssembly().Location.Split(dirChar).SkipLast(1).Append(resourcesDirName).Append(outputDirName));
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            string filePath = string.Join(dirChar, Assembly.GetExecutingAssembly().Location.Split(dirChar).SkipLast(1).Append(resourcesDirName).Append(outputDirName).Append(logName + fileExtension));
            var sw = File.CreateText(filePath);
            sw.Write(content);
            sw.Dispose();
        }

    }
}
