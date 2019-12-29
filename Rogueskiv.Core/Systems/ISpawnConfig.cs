using System.Collections.Generic;

namespace Rogueskiv.Core.Systems
{
    public interface ISpawnConfig
    {
        bool IsLastFloor { get; }

        int PlayerRadius { get; }
        int InitialPlayerHealth { get; }
        int InitialPlayerVisualRange { get; }
        float PlayerAcceleration { get; }
        float PlayerMaxSpeed { get; }
        float PlayerStopSpeed { get; }

        int EnemyNumber { get; }
        int EnemyCollisionDamage { get; }
        int MinEnemySpeed { get; }
        int MaxEnemySpeed { get; }
        List<(int numAngles, float weight)> EnemyNumAnglesProbWeights { get; }
        int MinSpaceToSpawnEnemy { get; }
        int MinEnemySpawnDistance { get; }
        int EnemyRadius { get; }

        float MinFoodSpawnDistanceFactor { get; }
        float MinTorchSpawnDistanceFactor { get; }
        float MinMapRevealerSpawnDistanceFactor { get; }
        float MinAmuletSpawnFactor { get; }
        float MinDownStairsSpawnFactor { get; }
        float PlayerBounceAmortiguationFactor { get; }
        float PlayerFrictionFactor { get; }

        int MaxItemPickingTime { get; }
        int FoodHealthIncrease { get; }
        int TorchVisualRangeIncrease { get; }
    }
}
