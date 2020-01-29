using System;
using System.Collections.Generic;

namespace Seedwork.Engine
{
    public class GameStages<T>
        : Dictionary<
            (GameStageCode, GameResultCode?),
            Func<IGameResult<T>, GameEngine<T>>
        >
    {
        public GameEngine<T> GetNext(GameStageCode stageCode, IGameResult<T> gameResult)
        {
            var nextStageKey = (stageCode, gameResult?.ResultCode);

            return ContainsKey(nextStageKey)
                ? this[nextStageKey](gameResult)
                : null;
        }
    }
}
