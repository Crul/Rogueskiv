using Seedwork.Core.Components;
using System;

namespace Rogueskiv.Core.Components
{
    public class HealthComp : IComponent
    {
        private int health;

        public int Health
        {
            get => health;
            set => health = Math.Min(value, MaxHealth);
        }

        public int MaxHealth { get; set; }

        public float HealthFactor { get => (float)Health / MaxHealth; }

        public bool Full { get => Health == MaxHealth; }
    }
}
