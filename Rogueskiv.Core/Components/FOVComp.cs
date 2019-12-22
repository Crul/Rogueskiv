using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using Rogueskiv.Core.Systems;
using Seedwork.Core.Components;
using Seedwork.Crosscutting;
using System.Linq;

namespace Rogueskiv.Core.Components
{
    public class FOVComp : IComponent
    {
        private const int VISUAL_RANGE = 10; // TODO proper visual range

        private FOVRecurse FOVRecurse;

        public void Init(BoardComp boardComp)
        {
            var size = BoardSys.GetSize(boardComp.Board);

            FOVRecurse = new FOVRecurse(size.Width, size.Height, VISUAL_RANGE);
            BoardSys.ForAllTiles(size, tilePos =>
                FOVRecurse.Point_Set(tilePos.X, tilePos.Y, !BoardSys.IsTile(boardComp.Board, tilePos) ? 1 : 0));
        }

        public void SetPlayerPos(PositionComp positionComp) =>
            FOVRecurse.SetPlayerPos(positionComp.TilePos.X, positionComp.TilePos.Y);

        public bool IsVisible(PositionComp positionComp) =>
            FOVRecurse.VisiblePoints.Any(vp => vp == positionComp.TilePos);
    }
}
