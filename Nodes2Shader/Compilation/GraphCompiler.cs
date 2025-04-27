using Nodes2Shader.Compilation.MathGraph;

namespace Nodes2Shader.Compilation
{
    public enum CompilationResult
    {
        Success,
        SomeGlslError
    }

    public static class GraphCompiler
    {
        public static string Compile(GraphData visualGraph, out CompilationResult result)
        {


            result = CompilationResult.Success;
            return "";
        }
    }
}
