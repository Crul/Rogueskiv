using Rogueskiv.Core.Components.Position;

namespace Rogueskiv.Core.Components
{
    public class StairsComp : CurrentPositionComp
    {
        // TODO DRY TileComp
        public bool HasBeenSeen { get; private set; }
        public bool VisibleByPlayer { get; private set; }
        public float DistanceFromPlayer { get; set; }

        public override bool Visible
        {
            get => HasBeenSeen;
            set
            {
                VisibleByPlayer = value;
                HasBeenSeen = HasBeenSeen || value;
            }
        }
    }
}
