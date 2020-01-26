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

        public float FloorFactor(int floor) =>
            (float)FloorCount == 1 ? 1f : (floor - 1) / (FloorCount - 1);

        public int GetEnemyNumber(float floorFactor) => GetFloorDependantValue1(EnemyNumberRange, floorFactor);

        public Range<float> GetEnemySpeedRangeInGameTicks(float floorFactor)
            => new Range<float>()
            {
                Start = GetSpeedInGameTicks(GetFloorDependantValue1(MinEnemySpeedRange, floorFactor)),
                End = GetSpeedInGameTicks(GetFloorDependantValue1(MaxEnemySpeedRange, floorFactor)),
            };

        public List<(int numAngles, float weight)> GetEnemyAnglesProbWeights(float floorFactor)
            => EnemyAnglesProbWeightRanges
                .Select(eapwr =>
                    (numAngles: eapwr.Value, weight: GetFloorDependantValue1(eapwr.WeightRange, floorFactor))
                )
                .ToList();

        public IMapGenerationParams GetMapGenerationParams(int floor)
        {
            var floorFactor = FloorFactor(floor);
            var mapSize = GetFloorDependantValue1(MapSizeRange, floorFactor);
            var corridorWidthProbWeights = CorridorWidthProbWeightRanges
                .Select(cwpwr => (numAngles: cwpwr.Value, weight: GetFloorDependantValue1(cwpwr.WeightRange, floorFactor)))
                .ToList();

            return new MapGenerationParams(
                width: mapSize,
                height: mapSize,
                GetFloorDependantValue1(RoomExpandProbRange, floorFactor),
                GetFloorDependantValue1(CorridorTurnProbRange, floorFactor),
                GetFloorDependantValue1(MinMapDensityRange, floorFactor),
                GetFloorDependantValue1(InitialRoomsRange, floorFactor),
                MinRoomSize,
                MinRoomSeparation,
                corridorWidthProbWeights
            );
        }

        private float GetSpeedInGameTicks(float speedInSeconds) => speedInSeconds / GameFPS;

        private int GetFloorDependantValue1(Range<int> range, float floorFactor) =>
            (int)(range.Start + (floorFactor * (range.End - range.Start)));

        private float GetFloorDependantValue1(Range<float> range, float floorFactor) =>
            (range.Start + (floorFactor * (range.End - range.Start)));
    }
}
