using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Seedwork.Crosscutting
{
    public static class YamlParser
    {
        public static T ParseFile<T>(string path, string filename)
            => ParseFile<T>(Path.Combine(path, $"{filename}.yaml"));

        public static T ParseFile<T>(string filePath)
        {
            if (!File.Exists(filePath))
                return default;

            using var fileReader = new StreamReader(filePath);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var data = deserializer.Deserialize<T>(fileReader);

            return data;
        }

        public static string Serialize<T>(T obj)
            => new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build()
                .Serialize(obj);
    }
}
