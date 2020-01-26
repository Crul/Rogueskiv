using Seedwork.Core.Components;
using System;
using System.Collections.Generic;

namespace Rogueskiv.Menus.MenuOptions
{
    class MenuOptionComp : IComponent
    {
        public int Order { get; }
        public Func<string> GetText { get; }
        public bool Focusable { get; }
        public IDictionary<Controls, Action> ActionsByControl { get; }
        public bool Active { get; set; }

        public MenuOptionComp(
            int order,
            Func<string> getTextFn,
            IDictionary<Controls, Action> actionsByControl = null
        )
        {
            Order = order;
            GetText = getTextFn;
            Focusable = actionsByControl != null;
            ActionsByControl = actionsByControl;
            Active = (order == 0);
        }
    }
}
