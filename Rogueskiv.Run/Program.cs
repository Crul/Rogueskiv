using System.IO;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Rogueskiv.Run
{
    class Program
    {
        private const string CONFIG_FILES_PATH = "config";
        private const string CONFIG_FILE_PATH_ARG = "--config=";
        private const string DEFAULT_CONFIG_FILE_PATH = "default";

        static void Main(string[] args)
        {
            var rogueskivConfig = ParseConfigFileArg(args);
            new RogueskivApp(rogueskivConfig).Run();
        }

        private static RogueskivAppConfig ParseConfigFileArg(string[] args)
        {
            var configFileName = args
                .Where(arg => arg.StartsWith(CONFIG_FILE_PATH_ARG))
                .Select(arg => arg.Substring(CONFIG_FILE_PATH_ARG.Length))
                .FirstOrDefault();

            if (string.IsNullOrEmpty(configFileName))
                configFileName = DEFAULT_CONFIG_FILE_PATH;

            var configFilePath = Path.Combine(CONFIG_FILES_PATH, $"{configFileName}.yaml");
            var fileReader = new StreamReader(configFilePath);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var rogueskivConfig = deserializer.Deserialize<RogueskivAppConfig>(fileReader);

            return rogueskivConfig;
        }
    }
}
