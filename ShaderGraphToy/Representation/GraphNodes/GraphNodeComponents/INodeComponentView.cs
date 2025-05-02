using Nodes2Shader.Compilation.MathGraph;

namespace ShaderGraphToy.Representation.GraphNodes.GraphNodeComponents
{
    public interface INodeComponentView
    {
        NodeEntry? GetData();
    }
}
