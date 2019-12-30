using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using Seedwork.Crosscutting;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Menus.MenuOptions
{
    class MenuSys : BaseSystem
    {
        private List<Controls> LastControls = new List<Controls>();

        public override void Update(EntityList entities, List<int> controls)
        {
            if (controls.Count == 0)
            {
                LastControls.Clear();
                return;
            }

            UpdateMenuOptions(entities, controls);

            LastControls = controls.Select(c => (Controls)c).ToList();
        }

        private void UpdateMenuOptions(EntityList entities, List<int> controls)
        {
            var menuOptions = entities.GetComponents<MenuOptionComp>()
                .OrderBy(mo => mo.Order)
                .ToList();

            var activeMenuOption = menuOptions.Single(mo => mo.Active);
            if (ControlPressed(controls, Controls.ENTER))
            {
                activeMenuOption.Execute(this);
                return;
            }

            int newActiveIndex;
            if (ControlPressed(controls, Controls.QUIT))
                newActiveIndex = menuOptions.Count - 1;

            else
            {
                var move = 0;
                if (ControlPressed(controls, Controls.UP))
                    move -= 1;
                if (ControlPressed(controls, Controls.DOWN))
                    move += 1;

                if (move == 0)
                    return;

                newActiveIndex = Maths.Modulo(
                    menuOptions.IndexOf(activeMenuOption) + move,
                    menuOptions.Count
                );
            }

            activeMenuOption.Active = false;
            menuOptions[newActiveIndex].Active = true;
        }

        private bool ControlPressed(List<int> controls, Controls control) =>
            controls.Contains((int)control) && !LastControls.Contains(control);
    }
}
