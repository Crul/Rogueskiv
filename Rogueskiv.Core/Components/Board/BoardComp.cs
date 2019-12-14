using Seedwork.Core.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Components.Board
{
    public class BoardComp : IComponent
    {
        public List<string> Board { get; set; }

        public BoardComp(string boardData) =>
            Board = boardData.Split(Environment.NewLine).ToList();

    }
}
