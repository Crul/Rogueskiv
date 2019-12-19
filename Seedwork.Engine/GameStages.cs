using System;
using System.Collections.Generic;

namespace Seedwork.Engine
{
    public class GameStages
        : Dictionary<
            (GameStageCode, GameResultCode?),
            Func<IGameResult, GameEngine>
        >
    {
        public GameEngine GetNext(GameStageCode stageCode, IGameResult gameResult)
        {
            var nextStageKey = (stageCode, gameResult?.ResultCode);

            return ContainsKey(nextStageKey)
                ? this[nextStageKey](gameResult)
                : null;
        }
    }
}
