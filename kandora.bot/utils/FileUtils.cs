using System.IO;
using System.Reflection;

namespace kandora.bot.utils
{
    public class FileUtils
    {

        public static string inputResourceLoc = "kandora.bot.resources";

        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            using StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static Stream GetStreamFromResourceFile(string fileName)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream($"{inputResourceLoc}.{fileName}");
        }

    }
}
