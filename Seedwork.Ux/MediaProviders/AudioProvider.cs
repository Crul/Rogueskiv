using SDL2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Seedwork.Ux.MediaProviders
{
    class AudioProvider
    {
        private readonly string AudiosPath;
        private readonly IDictionary<string, IntPtr> AudioChunksByPath;

        public AudioProvider(string audiosPath)
        {
            AudiosPath = audiosPath;
            AudioChunksByPath = new Dictionary<string, IntPtr>();
        }

        public IntPtr GetAudioChunk(string audioFile)
        {
            var audioPath = Path.Combine(AudiosPath, audioFile);
            if (!AudioChunksByPath.TryGetValue(audioPath, out var audioChunk))
            {
                audioChunk = SDL_mixer.Mix_LoadWAV(audioPath);
                AudioChunksByPath[audioPath] = audioChunk;
            }

            return audioChunk;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool cleanManagedResources)
        {
            if (cleanManagedResources)
            {
                AudioChunksByPath.Values.ToList().ForEach(SDL_mixer.Mix_FreeChunk);
                AudioChunksByPath.Clear();
            }
        }
    }
}
