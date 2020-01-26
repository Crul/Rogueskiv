using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Seedwork.Crosscutting
{
    public static class YamlParser
    {
        public static T ParseFile<T>(string path, string filename)
        {
            var filePath = Path.Combine(path, $"{filename}.yaml");
            using var fileReader = new StreamReader(filePath);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var data = deserializer.Deserialize<T>(fileReader);

            return data;
        }
    }
}
