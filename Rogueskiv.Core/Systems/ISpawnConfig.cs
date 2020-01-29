using Seedwork.Crosscutting;
using System.Collections.Generic;

namespace Rogueskiv.Core.Systems
{
    public interface ISpawnConfig
    {
        float FloorFactor(int floor);

        int PlayerRadius { get; }
        int InitialPlayerHealth { get; }
        float InitialPlayerVisualRange { get; }
        float PlayerAccelerationInGameTicks { get; }
        float PlayerMaxSpeedInGameTicks { get; }
        float PlayerStopSpeedInGameTicks { get; }

        int GetEnemyNumber(float floorFactor);
        int EnemyCollisionDamage { get; }
        float EnemyCollisionBounce { get; }
        Range<float> GetEnemySpeedRangeInGameTicks(float floorFactor);
        List<(int numAngles, float weight)> GetEnemyAnglesProbWeights(float floorFactor);
        int MinSpaceToSpawnEnemy { get; }
        int MinEnemySpawnDistance { get; }
        int EnemyRadius { get; }

        float MinFoodSpawnDistanceFactor { get; }
        float MinTorchSpawnDistanceFactor { get; }
        float MinMapRevealerSpawnDistanceFactor { get; }
        float MinAmuletSpawnFactor { get; }
        float MinDownStairsSpawnFactor { get; }
        float PlayerBounceMomentumConservationFactor { get; }
        float PlayerFrictionFactor { get; }

        int MaxItemPickingTimeInGameTicks { get; }
        int FoodHealthIncrease { get; }
        float TorchVisualRangeIncrease { get; }
    }
}
