using Rogueskiv.Core.Components;
using Rogueskiv.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    public class BoardSys : BaseSystem
    {
        public override bool Init(Game game)
        {
            var boardEntity = game
                .Entities
                .GetWithComponent<BoardComp>()
                .Single();

            var board = boardEntity.GetComponent<BoardComp>().Board;

            AddTiles(game, board);

            game.Entities.Remove(boardEntity);

            return false;
        }

        private void AddTiles(Game game, List<string> board)
        {
            var width = board[0].Length;
            var height = board.Count;
            for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                    if (IsTile(board, x, y))
                        game.AddEntity(new TileComp(x, y));
        }

        private bool IsTile(List<string> board, int x, int y) =>
            board[y].Substring(x, 1) == "T";

        public override void Update(List<IEntity> entities) { }
    }
}
