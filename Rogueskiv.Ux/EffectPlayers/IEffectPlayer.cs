using System;

namespace Rogueskiv.Ux.EffectPlayers
{
    interface IEffectPlayer : IDisposable
    {
        void Play();
    }
}
