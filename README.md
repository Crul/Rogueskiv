# Rogueskiv

### ⚠️ Work in progress: This is a proof of concept, not a fully developed game.

Rogueskiv is a mashup of [Eskiv](https://duckduckgo.com/?q=%22eskiv%22&t=ffab&ia=web) (by jf) and the countless [Roguelike games](https://www.reddit.com/r/roguelikedev/), inspired by the [Roguelike Celebration 2018](https://roguelike.club/event2018.html) talk: [Pippin Barr + Jonathan Lessard: Chess + Rogue = Chogue](https://www.youtube.com/watch?v=l1YEJBKehAY).

## Configuration

A configuration filename can be specified on execution (default value is `default`):

```Rogueskiv.Run.exe --config=configfilename```

Options will be loaded from `config/configfilename.yaml`.

## Environment Setup

Download and extract into `Seedwork.Ux/dlls/`:

- [(win32-x64) SDL2 2.0.10 Runtime Binaries](https://www.libsdl.org/release/SDL2-2.0.10-win32-x64.zip) at [libsdl.org](https://www.libsdl.org/download-2.0.php). File: SDL2.dll
- [(win32-x64) SDL_Image 2.0.5 Runtime Binaries](https://www.libsdl.org/projects/SDL_image/release/SDL2_image-2.0.5-win32-x64.zip) at [libsdl.org](https://www.libsdl.org/projects/SDL_image/). Files: SDL2_image.dll, libjpeg-9.dll, libpng16-16.dll, libtiff-5.dll, libwebp-7.dll, zlib1.dll.
- [(win32-x64) SDL_ttf 2.01.5 Runtime Binaries](https://www.libsdl.org/projects/SDL_ttf/release/SDL2_ttf-2.0.15-win32-x64.zip) at [libsdl.org](https://www.libsdl.org/projects/SDL_ttf/). Files: SDL2_ttf.dll, libfreetype-6.dll
- [(win32-x64) SDL_mixer 2.0.4 Runtime Binaries](https://www.libsdl.org/projects/SDL_mixer/release/SDL2_mixer-2.0.4-win32-x64.zip) at [libsdl.org](https://www.libsdl.org/projects/SDL_mixer/). Files: SDL2_mixer.dll, libFLAC-8.dll, libmodplug-1.dll, libmpg123-0.dll, libogg-0.dll, libopus-0.dll, libopusfile-0.dll, libvorbis-0.dll, libvorbisfile-3.dll

Tested only in Windows 10.

## Acknowledgements

Rogueskiv makes use of the following projects:

- [SDL 2.0](https://www.libsdl.org/index.php), see [credits](https://www.libsdl.org/credits.php) ([zlib license](https://www.libsdl.org/license.php))
- [SDL2-CS](https://github.com/flibitijibibo/SDL2-CS) by [Ethan Lee](http://www.flibitijibibo.com/) ([zlib license](https://github.com/flibitijibibo/SDL2-CS/blob/master/LICENSE))
- [FOVRecurse](https://github.com/AndyStobirski/RogueLike) by [Andy Stobirski](http://www.evilscience.co.uk/) (no license)
- [Hack typeface](https://sourcefoundry.org/hack/) by [Christopher Simpkins and contributors](https://github.com/source-foundry/Hack/blob/master/docs/CONTRIBUTORS.md) ([MIT license](https://github.com/source-foundry/Hack/blob/master/LICENSE.md))
- Part of the graphic tiles used in this program is the public domain roguelike tileset "RLTiles".  
  You can find the original tileset at: <http://rltiles.sf.net>
- [Stone Texture II](https://www.publicdomainpictures.net/en/view-image.php?image=67696&picture=stone-texture-ii) by [Daniel Smith](http://artistfire.deviantart.com/) ([CC0 Public Domain](https://www.publicdomainpictures.net/en/view-image.php?image=67696&picture=stone-texture-ii#image_text))
- [YamlDotNet](https://github.com/aaubry/YamlDotNet) by [Antoine Aubry](https://www.aaubry.net/) ([MIT License](https://github.com/aaubry/YamlDotNet/blob/master/LICENSE.txt))
- Sounds:
  - [two large cobblestone blocks](https://freesound.org/people/jobel0092/sounds/268055/) by [jobel0092](https://freesound.org/people/jobel0092/) ([Creative Commons 0 License](https://freesound.org/people/jobel0092/sounds/268055/#sound_license))
  - [Torch.wav](https://freesound.org/people/DanielVega/sounds/479338/) by [DanielVega](https://freesound.org/people/DanielVega/) ([Creative Commons 0 License](https://freesound.org/people/DanielVega/sounds/479338/#sound_license))

## License (AGPL-3.0)

Rogueskiv, a roguelike version of Eskiv  
Copyright (C) 2019, Raúl J. Vila

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.

See license.txt.
