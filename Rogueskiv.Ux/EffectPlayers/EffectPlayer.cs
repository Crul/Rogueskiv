using Rogueskiv.Core;
using Rogueskiv.Core.GameEvents;
using SDL2;
using System;
using System.Linq;

namespace Rogueskiv.Ux.EffectPlayers
{
    abstract class EffectPlayer<T> : IEffectPlayer
        where T : IGameEvent
    {
        private readonly IntPtr AudioChunk;
        private readonly int Channel;
        private readonly RogueskivGame Game;

        public EffectPlayer(RogueskivGame game, string audioFilePath, int channel = -1)
        {
            AudioChunk = SDL_mixer.Mix_LoadWAV(audioFilePath);
            Channel = channel;
            Game = game;
        }

        protected virtual int GetVolume(T gameEvent) => SDL_mixer.MIX_MAX_VOLUME;

        public void Play()
        {
            var gameEvents = Game.GameEvents.Where(ev => ev is T).ToList();
            if (gameEvents.Any())
                PlayEffect((T)gameEvents.First());
        }

        private void PlayEffect(T gameEvent)
        {
            SDL_mixer.Mix_VolumeChunk(AudioChunk, GetVolume(gameEvent));  // TODO only if volume has changed ?
            SDL_mixer.Mix_PlayChannel(Channel, AudioChunk, 0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool cleanManagedResources)
        {
            if (cleanManagedResources)
                SDL_mixer.Mix_FreeChunk(AudioChunk);
        }
    }
}
