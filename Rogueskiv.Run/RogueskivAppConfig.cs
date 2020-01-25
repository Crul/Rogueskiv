using Rogueskiv.Ux;
using System.Drawing;
using System.IO;

namespace Rogueskiv.Run
{
    class RogueskivAppConfig : IRogueskivUxConfig
    {
        public string GameModeFilesPath { get; set; }
        public string GameMode { get; set; }

        public int MinFloorCount { get; set; }
        public int MaxFloorCount { get; set; }
        public int FloorCount { get; set; }
        public string MenuMusicFilePath { get; set; }
        public int MenuMusicVolume { get; set; }

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
    }
}
