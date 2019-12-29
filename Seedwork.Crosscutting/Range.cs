using System;

namespace Seedwork.Crosscutting
{
    public class Range<T>
        where T : IComparable
    {
        public T Start { get; set; }
        public T End { get; set; }
    }
}
