using Seedwork.Core.Components;
using System;

namespace Rogueskiv.Menus.MenuOptions
{
    class MenuOptionComp : IComponent
    {
        public int Order { get; }
        public string Text { get; }
        public Action<MenuSys> Execute { get; }
        public bool Active { get; set; }

        public MenuOptionComp(int order, string text, Action<MenuSys> executeAction)
        {
            Order = order;
            Text = text;
            Execute = executeAction;
            Active = (order == 0);
        }
    }
}
