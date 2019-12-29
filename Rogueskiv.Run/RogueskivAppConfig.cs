﻿using Rogueskiv.Ux;
using Seedwork.Crosscutting;
using Seedwork.Ux;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Rogueskiv.Run
{
    class RogueskivAppConfig : IUxConfig, IRogueskivUxConfig
    {
        public int FloorCount { get; set; }

        // for GameContext
        public int MaxGameStepsWithoutRender { get; set; }

        // IUxConfig
        public bool Maximized { get; set; }
        public Size ScreenSize { get; set; }

        // IRogueskivUxConfig
        public string FontFile { get; } = Path.Join("fonts", "Hack", "Hack-Regular.ttf");
        public int CameraMovementFriction { get; set; }

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
        public int InitialPlayerVisualRange { get; set; }
        public float PlayerBounceAmortiguationFactor { get; set; }
        public float PlayerFrictionFactor { get; set; }
        public float PlayerAcceleration { get; set; }
        public float PlayerMaxSpeed { get; set; }
        public float PlayerStopSpeed { get; set; }

        public Range<int> EnemyNumberRange { get; set; }
        public int EnemyCollisionDamage { get; set; }
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

        public int MaxItemPickingTime { get; set; }
        public int FoodHealthIncrease { get; set; }
        public int TorchVisualRangeIncrease { get; set; }
    }
}