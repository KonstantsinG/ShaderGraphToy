using ShaderGraphToy.DllWrappers;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace ShaderGraphToy.Graphics
{
    public class RenderSurfaceHost : HwndHost
    {
        private readonly int _width = 800;
        private readonly int _height = 600;
        private IntPtr _renderWindowHandle = IntPtr.Zero;

        public int SurfaceId {  get; private set; }


        public RenderSurfaceHost(double width, double height)
        {
            _width = (int)width;
            _height = (int)height;
        }


        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            SurfaceId = ToyRendererAPI.CreateRenderSurface(hwndParent.Handle, _width, _height);
            _renderWindowHandle = ToyRendererAPI.GetHandle(SurfaceId);
            if (_renderWindowHandle == IntPtr.Zero)
                throw new ExternalException("Rendering window creation is failed");

            return new HandleRef(this, _renderWindowHandle);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            ToyRendererAPI.RemoveRenderSurface(SurfaceId);
            _renderWindowHandle = IntPtr.Zero;
        }
    }
}
