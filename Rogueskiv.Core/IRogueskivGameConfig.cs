using Rogueskiv.Core.Systems;
using Rogueskiv.MapGeneration;

namespace Rogueskiv.Core
{
    public interface IRogueskivGameConfig : ISpawnConfig
    {
        int Floor { get; }
        int GameSeed { get; }
        IMapGenerationParams MapGenerationParams { get; }
    }
}
