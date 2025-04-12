using System.Runtime.InteropServices;

namespace ShaderGraphToy.DllWrappers
{
    internal static class ToyRendererAPI
    {
        private const string _rendererDll = "ToyRendererDLL.dll";


        [DllImport(_rendererDll)]
        public static extern int CreateRenderSurface(IntPtr host, int width, int height);

        [DllImport(_rendererDll)]
        public static extern void RemoveRenderSurface(int id);

        [DllImport(_rendererDll)]
        public static extern IntPtr GetHandle(int id);
    }
}
