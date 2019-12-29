using Seedwork.Crosscutting;
using Seedwork.Ux;
using System.Collections.Generic;
using System.Drawing;

namespace Rogueskiv.Run
{
    class RogueskivConfig : IUxConfig
    {
        public int FloorCount { get; set; }

        // for IMapGenerationParams
        public int MinRoomSize { get; set; }
        public Range<int> MapSizeRange { get; set; }
        public Range<float> RoomExpandProbRange { get; set; }
        public Range<float> CorridorTurnProbRange { get; set; }
        public Range<float> MinMapDensityRange { get; set; }
        public Range<int> InitialRoomsRange { get; set; }
        public List<RangedWeightedValue<int>> CorridorWidthProbWeightRanges { get; set; }

        // ISpawnConfig
        public int PlayerRadius { get; set; }
        public int InitialPlayerHealth { get; set; }
        public int InitialPlayerVisualRange { get; set; }
        public float PlayerBounceAmortiguationFactor { get; set; }
        public float PlayerFrictionFactor { get; set; }

        public Range<int> EnemyNumberRange { get; set; }
        public Range<int> MinEnemySpeedRange { get; set; }
        public Range<int> MaxEnemySpeedRange { get; set; }
        public List<RangedWeightedValue<int>> EnemyAnglesProbWeightRanges { get; set; }

        public int MinSpaceToSpawnEnemy { get; set; }
        public int MinEnemySpawnDistance { get; set; }
        public int EnemyRadius { get; set; }
        public float MinFoodSpawnDistanceFactor { get; set; }
        public float MinTorchSpawnDistanceFactor { get; set; }
        public float MinMapRevealerSpawnDistanceFactor { get; set; }
        public float MinAmuletSpawnFactor { get; set; }
        public float MinDownStairsSpawnFactor { get; set; }

        // IUxConfig
        public bool Maximized { get; set; }
        public Size ScreenSize { get; set; }
    }
}
