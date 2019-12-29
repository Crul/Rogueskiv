using Rogueskiv.Core.Systems;
using Rogueskiv.MapGeneration;

namespace Rogueskiv.Core
{
    public interface IRogueskivGameConfig : ISpawnConfig
    {
        int Floor { get; }
        IMapGenerationParams MapGenerationParams { get; }
    }
}
