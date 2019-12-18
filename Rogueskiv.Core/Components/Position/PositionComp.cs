namespace Rogueskiv.Core.Components.Position
{
    public abstract class PositionComp : IPositionComp
    {
        public float X { get; set; }
        public float Y { get; set; }
        public virtual bool Visible { get; set; } = true;
    }
}
