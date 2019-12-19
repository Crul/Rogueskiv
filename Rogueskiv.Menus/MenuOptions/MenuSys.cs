using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Menus.MenuOptions
{
    class MenuSys : BaseSystem
    {
        private Game Game;
        private bool Moved = false;

        public override bool Init(Game game)
        {
            Game = game;

            return base.Init(game);
        }

        public override void Update(EntityList entities, IEnumerable<int> controls)
        {
            var controlList = controls.ToList();
            if (controlList.Count == 0)
            {
                Moved = false;
                return;
            }

            if (Moved) return;

            var menuOptions = entities
                .GetWithComponent<MenuOptionComp>()
                .Select(e => e.GetComponent<MenuOptionComp>())
                .OrderBy(mo => mo.Order)
                .ToList();

            var activeMenuOption = menuOptions.Single(mo => mo.Active);
            if (controlList.Contains((int)Controls.ENTER) || controlList.Contains((int)Controls.ENTER2))
            {
                Game.EndGame(activeMenuOption.Result);
                return;
            }

            var move = 0;
            if (controlList.Contains((int)Controls.UP))
                move -= 1;
            if (controlList.Contains((int)Controls.DOWN))
                move += 1;

            if (move == 0)
            {
                Moved = false;
                return;
            }

            var newActiveIndex = (
                    menuOptions.IndexOf(activeMenuOption) + move
                ) % menuOptions.Count;

            if (newActiveIndex < 0)
                newActiveIndex += menuOptions.Count;

            activeMenuOption.Active = false;
            menuOptions[newActiveIndex].Active = true;
            Moved = true;
        }
    }
}
