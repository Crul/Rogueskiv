using Rogueskiv.Core;
using Rogueskiv.MapGeneration;
using Seedwork.Crosscutting;
using Seedwork.Engine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Run
{
    class RogueskivGameConfig : IRogueskivGameConfig
    {
        private readonly RogueskivAppConfig RogueskivConfig;
        private readonly float FloorFactor;

        public int GameSeed { get; }
        public int Floor { get; }
        public bool IsLastFloor { get; }
        public IMapGenerationParams MapGenerationParams { get; }

        public int PlayerRadius => RogueskivConfig.PlayerRadius;
        public int InitialPlayerHealth => RogueskivConfig.InitialPlayerHealth;
        public float InitialPlayerVisualRange => RogueskivConfig.InitialPlayerVisualRange;
        public float PlayerBounceAmortiguationFactor => RogueskivConfig.PlayerBounceAmortiguationFactor;
        public float PlayerFrictionFactor => RogueskivConfig.PlayerFrictionFactor;
        public float PlayerAcceleration { get; }
        public float PlayerMaxSpeed { get; }
        public float PlayerStopSpeed { get; }

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

        public int EnemyCollisionDamage => RogueskivConfig.EnemyCollisionDamage;
        public float EnemyCollisionBounce => RogueskivConfig.EnemyCollisionBounce;
        public int MaxItemPickingTime { get; }
        public int FoodHealthIncrease => RogueskivConfig.FoodHealthIncrease;
        public float TorchVisualRangeIncrease => RogueskivConfig.TorchVisualRangeIncrease;

        public RogueskivGameConfig(RogueskivAppConfig rogueskivConfig, IGameContext gameContext, int floor)
        {
            RogueskivConfig = rogueskivConfig;
            GameSeed = gameContext.GameSeed;
            Floor = floor;
            IsLastFloor = floor == RogueskivConfig.FloorCount;
            FloorFactor = (float)floor / RogueskivConfig.FloorCount;

            EnemyNumber = GetFloorDependantValue(rogueskivConfig.EnemyNumberRange);
            MinEnemySpeed = GetFloorDependantValue(rogueskivConfig.MinEnemySpeedRange) / gameContext.GameFPS;
            MaxEnemySpeed = GetFloorDependantValue(rogueskivConfig.MaxEnemySpeedRange) / gameContext.GameFPS;
            MaxItemPickingTime = RogueskivConfig.MaxItemPickingTime / gameContext.GameFPS;

            // the parametrization is not perfect because with high FPS
            // there are more steps with movement for the same acceleration
            PlayerAcceleration = (float)Math.Pow(RogueskivConfig.PlayerAcceleration, 25d / gameContext.GameFPS);
            PlayerMaxSpeed = RogueskivConfig.PlayerMaxSpeed / gameContext.GameFPS;
            PlayerStopSpeed = RogueskivConfig.PlayerStopSpeed / gameContext.GameFPS;

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
            var minRoomSeparation = rogueskivConfig.MinRoomSeparation;
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
                minRoomSeparation,
                corridorWidthProbWeights
            );
        }

        private int GetFloorDependantValue(Range<int> range) =>
            (int)(range.Start + (FloorFactor * (range.End - range.Start)));

        private float GetFloorDependantValue(Range<float> range) =>
            (range.Start + (FloorFactor * (range.End - range.Start)));
    }
}
