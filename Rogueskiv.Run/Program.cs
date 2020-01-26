using Seedwork.Crosscutting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rogueskiv.Run
{
    class Program
    {
        private const string CONFIG_FILES_PATH = "config";
        private const string GAME_MODE_FILES_PATH = "gameModes";
        private const string CONFIG_FILE_NAME = "global";

        static void Main()
        {
            var rogueskivConfig = YamlParser.ParseFile<RogueskivAppConfig>(CONFIG_FILES_PATH, CONFIG_FILE_NAME);
            rogueskivConfig.GameModeFilesPath = Path.Combine(CONFIG_FILES_PATH, GAME_MODE_FILES_PATH);
            rogueskivConfig.GameModes = GetGameModes(rogueskivConfig);
            rogueskivConfig.CheckGameModeIndexBounds();

            using var rogueskivApp = new RogueskivApp(rogueskivConfig);
            rogueskivApp.Run();
        }

        private static List<string> GetGameModes(RogueskivAppConfig rogueskivConfig)
            => (new DirectoryInfo(rogueskivConfig.GameModeFilesPath)).GetFiles("*.yaml")
                .Select(fileInfo => Path.GetFileNameWithoutExtension(fileInfo.Name))
                .ToList();
    }
}
