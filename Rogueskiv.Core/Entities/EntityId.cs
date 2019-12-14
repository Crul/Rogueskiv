using System;

namespace Rogueskiv.Core.Entities
{
    public struct EntityId : IComparable
    {
        private readonly int _Value;

        public EntityId(int value) => _Value = value;

        public static implicit operator int(EntityId value) => value._Value;

        public override bool Equals(object obj) => obj.Equals(_Value);

        public override int GetHashCode() => _Value;

        public static bool operator ==(EntityId left, EntityId right) =>
            ((int)left) == right;

        public static bool operator !=(EntityId left, EntityId right) =>
            !(left == right);

        public override string ToString() => _Value.ToString();

        public int CompareTo(object obj) => _Value.CompareTo((EntityId)obj);

        public static bool operator <(EntityId left, EntityId right) =>
            left.CompareTo(right) < 0;

        public static bool operator <=(EntityId left, EntityId right) =>
            left.CompareTo(right) <= 0;

        public static bool operator >(EntityId left, EntityId right) =>
            left.CompareTo(right) > 0;

        public static bool operator >=(EntityId left, EntityId right) =>
            left.CompareTo(right) >= 0;
    }
}
