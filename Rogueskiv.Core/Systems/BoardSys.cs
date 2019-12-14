using Rogueskiv.Core.Components;
using Rogueskiv.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    public class BoardSys : BaseSystem
    {
        public override bool Init(List<IEntity> entities)
        {
            var boardEntity = entities
                .GetWithComponent<BoardComp>()
                .Single();

            var board = boardEntity.GetComponent<BoardComp>().Board;

            var tiles = new List<TileComp>();
            var width = board[0].Length;
            var height = board.Count;
            for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                    if (board[y].Substring(x, 1) == "T")
                        tiles.Add(new TileComp(x, y));

            // TODO add walls and Tiles
            var entityId = entities.Max(e => e.Id) + 1;
            foreach (var tile in tiles)
                entities.Add(new Entity(new EntityId(entityId++)).AddComponent(tile));

            entities.Remove(boardEntity);

            return false;
        }

        public override void Update(List<IEntity> entities) { }
    }
}
