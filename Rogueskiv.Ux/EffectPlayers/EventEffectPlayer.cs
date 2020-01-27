using Rogueskiv.Core;
using Rogueskiv.Core.GameEvents;
using SDL2;
using Seedwork.Ux;
using System.Linq;

namespace Rogueskiv.Ux.EffectPlayers
{
    abstract class EventEffectPlayer<T> : EffectPlayer
        where T : IGameEvent
    {
        private readonly RogueskivGame Game;

        protected EventEffectPlayer(UxContext uxContext, RogueskivGame game, string audioFilename)
            : base(uxContext, audioFilename) =>
            Game = game;

        protected virtual int GetVolume(T gameEvent) => SDL_mixer.MIX_MAX_VOLUME;

        public override void Play()
        {
            var gameEvents = Game.GameEvents.Where(ev => ev is T).ToList();
            if (gameEvents.Any())
                PlayEffect((T)gameEvents.First());
        }

        private void PlayEffect(T gameEvent) => PlayChunk(GetVolume(gameEvent));
    }
}
