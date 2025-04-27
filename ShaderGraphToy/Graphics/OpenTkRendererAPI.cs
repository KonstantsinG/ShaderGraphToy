

using ShaderGraphToy.Representation.Components;

namespace ShaderGraphToy.Graphics
{
    internal static class OpenTkRendererAPI
    {
        private static RenderingViewportVM? _renderer;

        internal static void RegisterRenderer(RenderingViewportVM rend) => _renderer = rend;
        internal static void ChangeFragmentShader(string shader) => _renderer?.ChangeFragmentShader(shader);
    }
}
