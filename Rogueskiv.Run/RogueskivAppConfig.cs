using Rogueskiv.Menus;
using Rogueskiv.Ux;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Rogueskiv.Run
{
    class RogueskivAppConfig : IRogueskivUxConfig, IRogueskivGameParams
    {
        public string GlobalConfigFilePath { get; internal set; }
        public string GameStatsFilePath { get; internal set; }
        public string GameModeFilesPath { get; set; }
        public string MenuMusicFilePath { get; set; }
        public int MenuMusicVolume { get; set; }

        // IRogueskivGameParams
        public int MinFloorCount { get; set; }
        public int MaxFloorCount { get; set; }
        public int FloorCount { get; set; }
        public List<string> GameModes { get; set; }
        public int GameModeIndex { get; set; }
        public string GameMode => GameModes[GameModeIndex];
        public int GameModesCount => GameModes.Count;

        // for GameContext
        public int MaxGameStepsWithoutRender { get; set; }

        // IUxConfig
        public bool Maximized { get; set; }
        public Size ScreenSize { get; set; }
        public bool SoundsOn { get; set; }
        public bool MusicOn { get; set; }

        // IRogueskivUxConfig
        public string FontFile { get; } = Path.Join("fonts", "Hack", "Hack-Regular.ttf");
        public int CameraMovementFriction { get; set; }

        public void ChangeFloorCount(int floorCountChange)
        {
            FloorCount += floorCountChange;
            FloorCount = Math.Min(MaxFloorCount, Math.Max(MinFloorCount, FloorCount));
        }

        public void ChangeGameMode(int gameModeChange)
        {
            GameModeIndex += gameModeChange;
            CheckGameModeIndexBounds();
        }

        public void CheckGameModeIndexBounds()
            => GameModeIndex = Math.Min(GameModesCount - 1, Math.Max(0, GameModeIndex));
    }
}
