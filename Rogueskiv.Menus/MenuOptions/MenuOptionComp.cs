using Seedwork.Core.Components;
using System;
using System.Collections.Generic;

namespace Rogueskiv.Menus.MenuOptions
{
    class MenuOptionComp : IComponent
    {
        public int Order { get; }
        public Func<string> GetText { get; }
        public IDictionary<Controls, Action> ActionsByControl { get; }
        public bool Active { get; set; }

        public MenuOptionComp(
            int order,
            Func<string> getTextFn,
            IDictionary<Controls, Action> actionsByControl
        )
        {
            Order = order;
            GetText = getTextFn;
            ActionsByControl = actionsByControl;
            Active = (order == 0);
        }
    }
}
