using SDL2;
using Seedwork.Crosscutting;
using Seedwork.Ux.MediaProviders;
using System;
using System.Drawing;

namespace Seedwork.Ux
{
    public class UxContext : IDisposable
    {
        public string Title { get; }
        public Size ScreenSize { get; private set; }
        public Point Center { get; set; }
        public IntPtr Window { get; }
        public IntPtr WRenderer { get; }

        private readonly IUxConfig UxConfig;
        private readonly TextureProvider TextureProvider;
        private readonly AudioProvider AudioProvider;
        private readonly FontProvider FontProvider;
        private IntPtr? MusicPointer = null;
        private int MusicVolume = 0;
        private string MusicFilePath;

        public UxContext(
            string windowTitle,
            IUxConfig uxConfig,
            string imagesPath,
            string audiosPath,
            string fontsPath
        )
        {
            Title = windowTitle;
            UxConfig = uxConfig;
            OnWindowResize(uxConfig.ScreenSize);

            var sdlWindowFlags = SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE;
            if (uxConfig.Maximized)
                sdlWindowFlags |= SDL.SDL_WindowFlags.SDL_WINDOW_MAXIMIZED;

            SDL.SDL_Init(SDL.SDL_INIT_VIDEO | SDL.SDL_INIT_AUDIO);
            SDL_ttf.TTF_Init();

            Window = SDL.SDL_CreateWindow(
                windowTitle,
                SDL.SDL_WINDOWPOS_CENTERED,
                SDL.SDL_WINDOWPOS_CENTERED,
                ScreenSize.Width, ScreenSize.Height,
                sdlWindowFlags
            );

            WRenderer = SDL.SDL_CreateRenderer(Window, -1, 0);
            SDL.SDL_SetRenderDrawColor(WRenderer, 0, 0, 0, 0);
            SDL.SDL_SetRenderDrawBlendMode(WRenderer, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);

            SDL_mixer.Mix_OpenAudio(44100, SDL_mixer.MIX_DEFAULT_FORMAT, 2, 2048);

            TextureProvider = new TextureProvider(WRenderer, imagesPath);
            AudioProvider = new AudioProvider(audiosPath);
            FontProvider = new FontProvider(fontsPath);
        }

        public IntPtr GetTexture(string imageFile) => TextureProvider.GetTexture(imageFile);

        public IntPtr GetAudioChunk(string audioFile) => AudioProvider.GetAudioChunk(audioFile);

        public IntPtr GetFont(string fontFile, int fontSize) => FontProvider.GetFont(fontFile, fontSize);

        public void OnWindowResize(int width, int height) =>
            OnWindowResize(new Size(width, height));

        private void OnWindowResize(Size screenSize)
        {
            Center = Center.Add(screenSize.Substract(ScreenSize).Divide(2).ToPoint());
            ScreenSize = screenSize;
        }

        public void PlayMusic(string musicFilePath, int volume)
        {
            if (MusicFilePath == musicFilePath)
                return;

            MusicFilePath = musicFilePath;
            MusicVolume = volume;

            DisposeMusic();
            if (UxConfig.MusicOn)
                LoadAndPlayMusic();
        }

        public void ToggleMusic()
        {
            if (UxConfig.MusicOn)
                SDL_mixer.Mix_HaltMusic();
            else
            {
                if (MusicPointer.HasValue)
                    SDL_mixer.Mix_PlayMusic(MusicPointer.Value, -1);
                else
                    LoadAndPlayMusic();
            }

            UxConfig.MusicOn = !UxConfig.MusicOn;
        }

        private void LoadAndPlayMusic()
        {
            MusicPointer = SDL_mixer.Mix_LoadMUS(MusicFilePath);
            SDL_mixer.Mix_FadeInMusic(MusicPointer.Value, -1, 1000);
            SDL_mixer.Mix_VolumeMusic(MusicVolume);
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
                TextureProvider.Dispose();
                DisposeMusic();
                SDL_mixer.Mix_Quit();
                SDL.SDL_DestroyRenderer(WRenderer);
                SDL.SDL_DestroyWindow(Window);
                SDL.SDL_Quit();
            }
        }

        private void DisposeMusic()
        {
            if (MusicPointer.HasValue)
            {
                SDL_mixer.Mix_FreeMusic(MusicPointer.Value);
                MusicPointer = null;
            }
        }
    }
}
