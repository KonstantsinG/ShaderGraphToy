using ShaderGraphToy.Representation.Components;

namespace ShaderGraphToy.Graphics
{
    internal static class OpenTkRendererAPI
    {
        private static string _fragmentCode = string.Empty;
        private static RenderingViewportVM? _renderer;

        public static string FragmentCode
        {
            get => _fragmentCode;
            set => _fragmentCode = value;
        }

        internal static void RegisterRenderer(RenderingViewportVM rend) => _renderer = rend;
        internal static void RenderFragmentShader(string shader, string[] uniforms)
        {
            FragmentCode = shader;
            _renderer?.ChangeFragmentShader(shader, uniforms);
        }
    }
}
