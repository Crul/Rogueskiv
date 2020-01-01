using SDL2;
using System;
using System.IO;

namespace Rogueskiv.Ux.EffectPlayers
{
    public abstract class EffectPlayer : IEffectPlayer
    {
        private readonly IntPtr AudioChunk;

        protected int Channel { get; set; } = -1;

        protected EffectPlayer(string audioFilename) =>
            AudioChunk = SDL_mixer.Mix_LoadWAV(Path.Combine("audio", $"{audioFilename}.mp3"));

        public abstract void Play();

        protected void PlayChunk(int volume, int loops = 0)
        {
            SetVolume(volume); // TODO only if volume has changed ?
            Channel = SDL_mixer.Mix_PlayChannel(-1, AudioChunk, loops);
            if (Channel < 0)
                Console.WriteLine("ERROR Mix_PlayChannel");
        }

        protected void SetVolume(int volume) =>
            SDL_mixer.Mix_VolumeChunk(AudioChunk, volume);

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
