using Seedwork.Core.Components;
using System;
using System.Collections.Generic;

namespace Rogueskiv.Menus
{
    class StatsComp : IComponent
    {
        private readonly Func<List<List<string>>> LoadStatsFn;

        public bool Visible { get; private set; }
        public int Page { get; set; }

        public List<List<string>> GameStats { get; set; }

        public StatsComp(Func<List<List<string>>> loadStatsFn) =>
            LoadStatsFn = loadStatsFn;

        public void Show()
        {
            Visible = true;
            Page = 0;
            GameStats = LoadStatsFn();
        }

        public void Hide() => Visible = false;
    }
}
