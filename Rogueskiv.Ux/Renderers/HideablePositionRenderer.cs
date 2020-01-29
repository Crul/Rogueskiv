using Rogueskiv.Core.Components.Position;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Ux;
using Seedwork.Ux.SpriteProviders;

namespace Rogueskiv.Ux.Renderers
{
    class HideablePositionRenderer<T> : PositionRenderer<T>
        where T : IPositionComp
    {
        public HideablePositionRenderer(
            UxContext uxContext,
            IRenderizable game,
            ISpriteProvider<T> spriteProvider
        ) : base(uxContext, game, spriteProvider)
        { }

        protected override void Render(IEntity entity, T positionComp, float interpolation)
        {
            if (FOVComp.HasBeenSeenOrRevealed(positionComp))
                base.Render(entity, positionComp, interpolation);
        }
    }
}
