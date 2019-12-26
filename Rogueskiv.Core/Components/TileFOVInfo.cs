using Rogueskiv.Core.Components.Position;
using System.Drawing;

namespace Rogueskiv.Core.Components
{
    public class TileFOVInfo : PositionComp // TODO is this a comp ?
    {
        public bool VisibleByPlayer { get; set; }

        public float DistanceFactor { get; set; }

        public bool HasBeenSeen { get; set; }

        public bool CoveredByFOV
        {
            get => Visible && !VisibleByPlayer;
        }

        public bool Visible
        {
            get => HasBeenSeen;
            set
            {
                VisibleByPlayer = value;
                HasBeenSeen = HasBeenSeen || value;
                revealedByMap = revealedByMap && !value;
            }
        }

        private bool revealedByMap;
        public bool RevealedByMap
        {
            get => revealedByMap;
            private set
            {
                revealedByMap = value && !HasBeenSeen;
                HasBeenSeen = HasBeenSeen || value;
            }
        }

        public TileFOVInfo(int x, int y) : base(new Point(x, y)) { }

        public void Reveal() => RevealedByMap = true;
    }
}
