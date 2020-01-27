using SDL2;
using Seedwork.Ux;
using System;

namespace Rogueskiv.Ux.EffectPlayers
{
    public abstract class EffectPlayer : IEffectPlayer
    {
        private readonly IntPtr AudioChunk;

        protected int Channel { get; set; } = -1;

        protected EffectPlayer(UxContext uxContext, string audioFilename) =>
            AudioChunk = uxContext.GetAudioChunk($"{audioFilename}.mp3");

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
        { }
    }
}
