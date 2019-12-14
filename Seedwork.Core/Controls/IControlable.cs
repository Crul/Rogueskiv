using System.Collections.Generic;

namespace Seedwork.Core.Controls
{
    public interface IControlable
    {
        IEnumerable<int> Controls { get; set; }
    }
}
