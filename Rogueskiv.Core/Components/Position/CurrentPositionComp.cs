using System.Drawing;

namespace Rogueskiv.Core.Components.Position
{
    public class CurrentPositionComp : PositionComp
    {
        public bool HasBeenSeen { get; set; }
        public bool VisibleByPlayer { get; protected set; }
        public override bool Visible
        {
            get => HasBeenSeen;
            set
            {
                VisibleByPlayer = value;
                HasBeenSeen = HasBeenSeen || value;
            }
        }

        public CurrentPositionComp(PointF position) : base(position)
        { }
    }
}
