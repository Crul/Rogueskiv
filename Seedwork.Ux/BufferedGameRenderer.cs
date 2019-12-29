using Seedwork.Core;
using Seedwork.Ux.Renderers;

namespace Seedwork.Ux
{
    public class BufferedGameRenderer : GameRenderer
    {
        private readonly BufferRenderer BufferRenderer;

        public BufferedGameRenderer(UxContext uxContext, IRenderizable game)
            : base(uxContext, game) =>
            BufferRenderer = new BufferRenderer(uxContext);

        protected override void RenderGame(float interpolation)
        {
            BufferRenderer.ResetBuffer();
            base.RenderGame(interpolation);
            BufferRenderer.Render();
        }

        public override void OnWindowResize() => BufferRenderer.OnWindowResize();

        protected override void Dispose(bool cleanManagedResources)
        {
            base.Dispose(cleanManagedResources);
            if (cleanManagedResources)
                BufferRenderer.Dispose();
        }
    }
}
