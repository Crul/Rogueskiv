using System;

namespace Seedwork.Engine
{
    public struct GameResultCode : IComparable
    {
        private readonly int _Value;

        public GameResultCode(int value) => _Value = value;

        public static implicit operator int(GameResultCode value) => value._Value;

        public override bool Equals(object obj) => obj.Equals(_Value);

        public override int GetHashCode() => _Value;

        public static bool operator ==(GameResultCode left, GameResultCode right) =>
            ((int)left) == right;

        public static bool operator !=(GameResultCode left, GameResultCode right) =>
            !(left == right);

        public override string ToString() => _Value.ToString();

        public int CompareTo(object obj) => _Value.CompareTo((GameResultCode)obj);

        public static bool operator <(GameResultCode left, GameResultCode right) =>
            left.CompareTo(right) < 0;

        public static bool operator <=(GameResultCode left, GameResultCode right) =>
            left.CompareTo(right) <= 0;

        public static bool operator >(GameResultCode left, GameResultCode right) =>
            left.CompareTo(right) > 0;

        public static bool operator >=(GameResultCode left, GameResultCode right) =>
            left.CompareTo(right) >= 0;
    }
}
