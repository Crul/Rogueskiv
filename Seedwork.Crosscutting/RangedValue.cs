using System;

namespace Seedwork.Crosscutting
{
    public class RangedWeightedValue<T>
        where T : IComparable
    {
        public T Value { get; set; }
        public Range<float> WeightRange { get; set; }
    }
}
