using Seedwork.Crosscutting;
using System.IO;

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

            using var rogueskivApp = new RogueskivApp(rogueskivConfig);
            rogueskivApp.Run();
        }
    }
}
