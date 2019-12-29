using Seedwork.Core.Components;

namespace Rogueskiv.Core.Components
{
    public class PlayerComp : IComponent
    {
        public const int PLAYER_RADIUS = 10;
        public const int INITIAL_PLAYER_HEALTH = 100;
        private const int INITIAL_VISUAL_RANGE = 10;

        public int VisualRange { get; set; } = INITIAL_VISUAL_RANGE;
    }
}
