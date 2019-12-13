using System.Collections.Generic;

namespace Rogueskiv.Core.Controls
{
    public interface IControlable
    {
        IEnumerable<Control> Controls { get; set; }
    }
}
