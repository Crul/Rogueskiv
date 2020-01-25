using Rogueskiv.Core;
using Rogueskiv.MapGeneration;
using Seedwork.Crosscutting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Run
{
    class RogueskivGameConfig : IRogueskivGameConfig
    {
        public int GameSeed { get; set; }
        public int FloorCount { get; set; }
        public int GameFPS { get; set; }

        public string GameMusicFilePath { get; set; }
        public int GameMusicVolume { get; set; }
        public bool InGameTimeVisible { get; set; }
        public bool RealTimeVisible { get; set; }

        // for IMapGenerationParams
        public int MinRoomSize { get; set; }
        public int MinRoomSeparation { get; set; }
        public Range<int> MapSizeRange { get; set; }
        public Range<float> RoomExpandProbRange { get; set; }
        public Range<float> CorridorTurnProbRange { get; set; }
        public Range<float> MinMapDensityRange { get; set; }
        public Range<int> InitialRoomsRange { get; set; }
        public List<RangedWeightedValue<int>> CorridorWidthProbWeightRanges { get; set; }

        // ISpawnConfig
        public int PlayerRadius { get; set; }
        public int InitialPlayerHealth { get; set; }
        public float InitialPlayerVisualRange { get; set; }
        public float PlayerBounceAmortiguationFactor { get; set; }
        public float PlayerFrictionFactor { get; set; }
        public float PlayerAcceleration { get; set; }
        public float PlayerMaxSpeed { get; set; }
        public float PlayerStopSpeed { get; set; }

        public Range<int> EnemyNumberRange { get; set; }
        public Range<float> MinEnemySpeedRange { get; set; }
        public Range<float> MaxEnemySpeedRange { get; set; }
        public List<RangedWeightedValue<int>> EnemyAnglesProbWeightRanges { get; set; }
        public int MinSpaceToSpawnEnemy { get; set; }
        public int MinEnemySpawnDistance { get; set; }
        public int EnemyRadius { get; set; }
        public float MinFoodSpawnDistanceFactor { get; set; }
        public float MinTorchSpawnDistanceFactor { get; set; }
        public float MinMapRevealerSpawnDistanceFactor { get; set; }
        public float MinAmuletSpawnFactor { get; set; }
        public float MinDownStairsSpawnFactor { get; set; }

        public int EnemyCollisionDamage { get; set; }
        public float EnemyCollisionBounce { get; set; }
        public float MaxItemPickingTime { get; set; }
        public int FoodHealthIncrease { get; set; }
        public float TorchVisualRangeIncrease { get; set; }

        // the parametrization is not perfect because with high FPS
        // there are more steps with movement for the same acceleration
        public float PlayerAccelerationInGameTicks => (float)Math.Pow(PlayerAcceleration, 25d / GameFPS);

        public float PlayerMaxSpeedInGameTicks => GetSpeedInGameTicks(PlayerMaxSpeed);
        public float PlayerStopSpeedInGameTicks => GetSpeedInGameTicks(PlayerStopSpeed);
        public int MaxItemPickingTimeInGameTicks => (int)(MaxItemPickingTime * GameFPS);

        public bool IsLastFloor(int floor) => FloorFactor(floor) == 1f;

        public int GetEnemyNumber(int floor) => GetFloorDependantValue(EnemyNumberRange, floor);

        public Range<float> GetEnemySpeedRangeInGameTicks(int floor)
            => new Range<float>()
            {
                Start = GetSpeedInGameTicks(GetFloorDependantValue(MinEnemySpeedRange, floor)),
                End = GetSpeedInGameTicks(GetFloorDependantValue(MaxEnemySpeedRange, floor)),
            };

        public List<(int numAngles, float weight)> GetEnemyAnglesProbWeights(int floor)
            => EnemyAnglesProbWeightRanges
                .Select(eapwr => (numAngles: eapwr.Value, weight: GetFloorDependantValue(eapwr.WeightRange, floor)))
                .ToList();

        public IMapGenerationParams GetMapGenerationParams(int floor)
        {
            var mapSize = GetFloorDependantValue(MapSizeRange, floor);
            var corridorWidthProbWeights = CorridorWidthProbWeightRanges
                .Select(cwpwr => (numAngles: cwpwr.Value, weight: GetFloorDependantValue(cwpwr.WeightRange, floor)))
                .ToList();

            return new MapGenerationParams(
                width: mapSize,
                height: mapSize,
                GetFloorDependantValue(RoomExpandProbRange, floor),
                GetFloorDependantValue(CorridorTurnProbRange, floor),
                GetFloorDependantValue(MinMapDensityRange, floor),
                GetFloorDependantValue(InitialRoomsRange, floor),
                MinRoomSize,
                MinRoomSeparation,
                corridorWidthProbWeights
            );
        }

        private float GetSpeedInGameTicks(float speedInSeconds) => speedInSeconds / GameFPS;

        private int GetFloorDependantValue(Range<int> range, int floor) =>
            (int)(range.Start + (FloorFactor(floor) * (range.End - range.Start)));

        private float GetFloorDependantValue(Range<float> range, int floor) =>
            (range.Start + (FloorFactor(floor) * (range.End - range.Start)));

        private float FloorFactor(int floor) => (float)floor / FloorCount;
    }
}
