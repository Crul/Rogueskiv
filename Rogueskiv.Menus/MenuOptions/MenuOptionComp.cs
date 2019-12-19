using Seedwork.Core.Components;
using Seedwork.Core.Entities;
using Seedwork.Engine;

namespace Rogueskiv.Menus.MenuOptions
{
    class MenuOptionComp : IComponent
    {
        public int Order { get; }
        public string Text { get; }
        public IGameResult<IEntity> Result { get; }
        public bool Active { get; set; }

        public MenuOptionComp(int order, string text, IGameResult<IEntity> result)
        {
            Order = order;
            Text = text;
            Result = result;
            Active = (order == 0);
        }
    }
}
