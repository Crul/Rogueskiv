using System.Drawing;

namespace Rogueskiv.Core.Components.Position
{
    public abstract class RevealableByMapPositionComp : CurrentPositionComp
    {
        private bool revealedByMap;
        public bool RevealedByMap
        {
            get => revealedByMap;
            set
            {
                revealedByMap = value && !HasBeenSeen;
                HasBeenSeen = HasBeenSeen || value;
            }
        }

        public override bool Visible
        {
            get => HasBeenSeen;
            set
            {
                VisibleByPlayer = value;
                HasBeenSeen = HasBeenSeen || value;
                revealedByMap = revealedByMap && !value;
            }
        }

        protected RevealableByMapPositionComp(PointF position) : base(position)
        { }
    }
}
