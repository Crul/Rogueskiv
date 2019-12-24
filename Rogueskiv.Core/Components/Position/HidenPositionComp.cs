using System.Drawing;

namespace Rogueskiv.Core.Components.Position
{
    class HidenPositionComp : CurrentPositionComp
    {
        private bool visible;
        public override bool Visible
        {
            get => visible;
            set => visible = value;
        }

        public HidenPositionComp(PointF position) : base(position)
        { }
    }
}
