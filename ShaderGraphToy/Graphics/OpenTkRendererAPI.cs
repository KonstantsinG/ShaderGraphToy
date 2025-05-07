using ShaderGraphToy.Representation.Components;

namespace ShaderGraphToy.Graphics
{
    internal static class OpenTkRendererAPI
    {
        private static RenderingViewportVM? _renderer;

        internal static void RegisterRenderer(RenderingViewportVM rend) => _renderer = rend;
        internal static void RenderFragmentShader(string shader, string[] uniforms) => _renderer?.ChangeFragmentShader(shader, uniforms);
    }
}
