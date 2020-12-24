using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TransferProtocolLibrary.Config.Parsers;

namespace TransferProtocolLibrary.Config
{
    public static class ConfigManager
    {
        public static T GetOptions<T>(string directory, string filePattern)
        {
            foreach (string file in Directory.GetFiles(directory, filePattern))
            {
                List<KeyValuePair<string, object>> settings;
                string extension = Path.GetExtension(file).ToLower();
                IParser parser;

                switch (extension)
                {
                    case ".json":
                        using (StreamReader reader = new StreamReader(file, Encoding.UTF8, true))
                        {
                            parser = new JsonParser(reader.ReadToEnd());
                        }
                        break;
                    case ".xml":
                        using (StreamReader reader = new StreamReader(file, Encoding.UTF8, true))
                        {
                            parser = new XmlParser(reader.ReadToEnd());
                        }
                        break;
                    default:
                        continue;
                }
                try
                {
                    settings = parser.CreateTree();
                    object result = ClassGenerator.ParseFromTree(typeof(T), settings);
                    return (T)result;
                }
                catch (Exception)
                {
                    throw;
                }
            }

            throw new FileNotFoundException();
        }
    }
}
