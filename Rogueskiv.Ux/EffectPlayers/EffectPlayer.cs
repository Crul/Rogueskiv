using SDL2;
using System;
using System.IO;

namespace Rogueskiv.Ux.EffectPlayers
{
    public abstract class EffectPlayer : IEffectPlayer
    {
        private readonly IntPtr AudioChunk;
        private int Channel;

        protected EffectPlayer(string audioFilename, int channel = -1)
        {
            AudioChunk = SDL_mixer.Mix_LoadWAV(Path.Combine("audio", $"{audioFilename}.mp3"));
            Channel = channel;
        }

        public abstract void Play();

        protected void PlayChunk(int volume)
        {
            SetVolume(volume); // TODO only if volume has changed ?
            Channel = SDL_mixer.Mix_PlayChannel(Channel, AudioChunk, 0);
        }

        protected void PauseChunk()
        {
            if (Channel < 0)
                return;

            var isPaused = SDL_mixer.Mix_Paused(Channel) == 1;
            if (!isPaused)
                SDL_mixer.Mix_Pause(Channel);
        }

        protected void SetVolume(int volume) =>
            SDL_mixer.Mix_VolumeChunk(AudioChunk, volume);

        protected bool IsPlaying() =>
            Channel >= 0
            && SDL_mixer.Mix_Playing(Channel) == 1
            && SDL_mixer.Mix_Paused(Channel) == 0;

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
