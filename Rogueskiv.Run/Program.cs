using Seedwork.Crosscutting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rogueskiv.Run
{
    static class Program
    {
        private const string DATA_FILES_PATH = "data";
        private const string APP_DATA_FOLDER = "Rogueskiv";
        private const string GAME_MODE_FILES_PATH = "gameModes";
        private const string CONFIG_FILE_NAME = "config";
        private const string GAME_STATS_FILE_NAME = "stats";

        static void Main()
        {
            while (true)
            {
                var map = MapGeneration.MapGenerator.GenerateMap(new MapGeneration.MapGenerationParams(
                    width: 48,
                    height: 48,
                    roomExpandProbability: 0.0f,
                    corridorTurnProbability: 0.1f,
                    minDensity: 0f,
                    initialRooms: 150,
                    minRoomSize: 0,
                    minRoomSeparation: 2,
                    corridorWidthProbWeights: new List<(int width, float weight)>()
                    {
                        (1, 1)
                    })
                );
                if (string.IsNullOrEmpty(map))
                    Console.Write(".");
                else
                {
                    Console.WriteLine("\nMAP");
                    Console.WriteLine(map.ToUpper().Replace("T", " ").Replace(".", "#"));
                    Console.ReadLine();
                }

            }

            var rogueskivConfig = GetRogueskivAppConfig();
            using var rogueskivApp = new RogueskivApp(rogueskivConfig);
            rogueskivApp.Run();
        }

        private static RogueskivAppConfig GetRogueskivAppConfig()
        {
            var dataFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                APP_DATA_FOLDER
            );
            CheckApplicationDataFolder(dataFilePath);

            var rogueskivConfig = YamlParser.ParseFile<RogueskivAppConfig>(dataFilePath, CONFIG_FILE_NAME);
            rogueskivConfig.GlobalConfigFilePath = Path.Combine(dataFilePath, $"{CONFIG_FILE_NAME}.yaml");
            rogueskivConfig.GameStatsFilePath = Path.Combine(dataFilePath, $"{GAME_STATS_FILE_NAME}.yaml");
            rogueskivConfig.GameModeFilesPath = Path.Combine(dataFilePath, GAME_MODE_FILES_PATH);
            rogueskivConfig.GameModes = GetGameModes(rogueskivConfig);
            rogueskivConfig.CheckGameModeIndexBounds();

            return rogueskivConfig;
        }

        private static List<string> GetGameModes(RogueskivAppConfig rogueskivConfig)
            => (new DirectoryInfo(rogueskivConfig.GameModeFilesPath)).GetFiles("*.yaml")
                .Select(fileInfo => Path.GetFileNameWithoutExtension(fileInfo.Name))
                .ToList();

        private static void CheckApplicationDataFolder(string dataFilePath)
        {
            if (Directory.Exists(dataFilePath))
                return;

            Directory.CreateDirectory(dataFilePath);
            FileCopy.DirectoryCopy(DATA_FILES_PATH, dataFilePath, copySubDirs: true);
        }
    }
}
