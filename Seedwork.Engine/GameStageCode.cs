using System;

namespace Seedwork.Engine
{
    public struct GameStageCode : IComparable
    {
        private readonly int _Value;

        public GameStageCode(int value) => _Value = value;

        public static implicit operator int(GameStageCode value) => value._Value;

        public override bool Equals(object obj) => obj.Equals(_Value);

        public override int GetHashCode() => _Value;

        public static bool operator ==(GameStageCode left, GameStageCode right) =>
            ((int)left) == right;

        public static bool operator !=(GameStageCode left, GameStageCode right) =>
            !(left == right);

        public override string ToString() => _Value.ToString();

        public int CompareTo(object obj) => _Value.CompareTo((GameStageCode)obj);

        public static bool operator <(GameStageCode left, GameStageCode right) =>
            left.CompareTo(right) < 0;

        public static bool operator <=(GameStageCode left, GameStageCode right) =>
            left.CompareTo(right) <= 0;

        public static bool operator >(GameStageCode left, GameStageCode right) =>
            left.CompareTo(right) > 0;

        public static bool operator >=(GameStageCode left, GameStageCode right) =>
            left.CompareTo(right) >= 0;
    }
}
