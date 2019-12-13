namespace Rogueskiv.Core.Components
{
    public class PositionComp : IComponent
    {
        public float X { get; set; }
        public float Y { get; set; }

        internal PositionComp Clone() => new PositionComp() { X = X, Y = Y };
    }
}
