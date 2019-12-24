﻿using Rogueskiv.Core.Components.Position;
using System.Drawing;

namespace Rogueskiv.Core.Components
{
    public abstract class StairsComp : CurrentPositionComp
    {
        // TODO DRY TileComp
        public bool HasBeenSeen { get; set; }
        public bool VisibleByPlayer { get; private set; }

        private bool revealedByMap;
        public bool RevealedByMap
        {
            get => revealedByMap;
            set
            {
                revealedByMap = value && !HasBeenSeen;
                HasBeenSeen = HasBeenSeen || value;
            }
        }

        public override bool Visible
        {
            get => HasBeenSeen;
            set
            {
                VisibleByPlayer = value;
                HasBeenSeen = HasBeenSeen || value;
            }
        }

        protected StairsComp(PointF position) : base(position)
        { }
    }
}
