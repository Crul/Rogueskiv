using Rogueskiv.Core.Systems;
using Rogueskiv.MapGeneration;

namespace Rogueskiv.Core
{
    public interface IRogueskivGameConfig : ISpawnConfig
    {
        int GameFPS { get; }
        int GameSeed { get; }
        bool InGameTimeVisible { get; }
        bool RealTimeVisible { get; }
        IMapGenerationParams GetMapGenerationParams(int floor);
    }
}
