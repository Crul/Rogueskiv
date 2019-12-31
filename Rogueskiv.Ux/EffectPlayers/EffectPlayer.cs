using SDL2;
using System;
using System.IO;

namespace Rogueskiv.Ux.EffectPlayers
{
    public abstract class EffectPlayer : IEffectPlayer
    {
        private readonly IntPtr AudioChunk;
        private readonly int Channel;

        protected EffectPlayer(string audioFilename, int channel = -1)
        {
            AudioChunk = SDL_mixer.Mix_LoadWAV(Path.Combine("audio", $"{audioFilename}.mp3"));
            Channel = channel;
        }

        public abstract void Play();

        protected void PlayChunk(int volume)
        {
            SDL_mixer.Mix_VolumeChunk(AudioChunk, volume);  // TODO only if volume has changed ?
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
