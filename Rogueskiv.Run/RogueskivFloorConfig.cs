using Rogueskiv.Core;
using Rogueskiv.MapGeneration;
using Seedwork.Crosscutting;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Run
{
    class RogueskivFloorConfig : IRogueskivGameConfig
    {
        private readonly RogueskivConfig RogueskivConfig;
        private readonly float FloorFactor;

        public int Floor { get; }
        public bool IsLastFloor { get; }
        public IMapGenerationParams MapGenerationParams { get; }

        public int PlayerRadius => RogueskivConfig.PlayerRadius;
        public int InitialPlayerHealth => RogueskivConfig.InitialPlayerHealth;
        public int InitialPlayerVisualRange => RogueskivConfig.InitialPlayerVisualRange;
        public float PlayerBounceAmortiguationFactor => RogueskivConfig.PlayerBounceAmortiguationFactor;
        public float PlayerFrictionFactor => RogueskivConfig.PlayerFrictionFactor;

        public int EnemyNumber { get; }
        public int MinEnemySpeed { get; }
        public int MaxEnemySpeed { get; }
        public List<(int numAngles, float weight)> EnemyNumAnglesProbWeights { get; }
        public int MinSpaceToSpawnEnemy => RogueskivConfig.MinSpaceToSpawnEnemy;
        public int MinEnemySpawnDistance => RogueskivConfig.MinEnemySpawnDistance;
        public int EnemyRadius => RogueskivConfig.EnemyRadius;
        public float MinFoodSpawnDistanceFactor => RogueskivConfig.MinFoodSpawnDistanceFactor;
        public float MinTorchSpawnDistanceFactor => RogueskivConfig.MinTorchSpawnDistanceFactor;
        public float MinMapRevealerSpawnDistanceFactor => RogueskivConfig.MinMapRevealerSpawnDistanceFactor;
        public float MinAmuletSpawnFactor => RogueskivConfig.MinAmuletSpawnFactor;
        public float MinDownStairsSpawnFactor => RogueskivConfig.MinDownStairsSpawnFactor;

        public RogueskivFloorConfig(RogueskivConfig rogueskivConfig, int floor)
        {
            RogueskivConfig = rogueskivConfig;
            Floor = floor;
            IsLastFloor = floor == RogueskivConfig.FloorCount;
            FloorFactor = (float)floor / RogueskivConfig.FloorCount;

            EnemyNumber = GetFloorDependantValue(rogueskivConfig.EnemyNumberRange);
            MinEnemySpeed = GetFloorDependantValue(rogueskivConfig.MinEnemySpeedRange);
            MaxEnemySpeed = GetFloorDependantValue(rogueskivConfig.MaxEnemySpeedRange);

            EnemyNumAnglesProbWeights = rogueskivConfig
                .EnemyAnglesProbWeightRanges
                .Select(eapwr => (numAngles: eapwr.Value, weight: GetFloorDependantValue(eapwr.WeightRange)))
                .ToList();

            var mapSize = GetFloorDependantValue(rogueskivConfig.MapSizeRange);
            var roomExpandProb = GetFloorDependantValue(rogueskivConfig.RoomExpandProbRange);
            var corridorTurnProb = GetFloorDependantValue(rogueskivConfig.CorridorTurnProbRange);
            var minMapDensity = GetFloorDependantValue(rogueskivConfig.MinMapDensityRange);
            var initialRooms = GetFloorDependantValue(rogueskivConfig.InitialRoomsRange);
            var minRoomSize = rogueskivConfig.MinRoomSize;
            var corridorWidthProbWeights = rogueskivConfig
                .CorridorWidthProbWeightRanges
                .Select(cwpwr => (numAngles: cwpwr.Value, weight: GetFloorDependantValue(cwpwr.WeightRange)))
                .ToList();

            MapGenerationParams = new MapGenerationParams(
                width: mapSize,
                height: mapSize,
                roomExpandProb,
                corridorTurnProb,
                minMapDensity,
                initialRooms,
                minRoomSize,
                corridorWidthProbWeights
            );
        }

        private int GetFloorDependantValue(Range<int> range) =>
            (int)(range.Start + (FloorFactor * (range.End - range.Start)));

        private float GetFloorDependantValue(Range<float> range) =>
            (range.Start + (FloorFactor * (range.End - range.Start)));
    }
}
