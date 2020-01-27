namespace Rogueskiv.Menus
{
    public interface IRogueskivGameParams
    {
        int MinFloorCount { get; }
        int MaxFloorCount { get; }
        int FloorCount { get; set; }

        string GameMode { get; }
        int GameModeIndex { get; }
        int GameModesCount { get; }

        void ChangeFloorCount(int floorCountChange);
        void ChangeGameMode(int gameModeChange);
    }
}
