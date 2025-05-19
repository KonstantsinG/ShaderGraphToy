using Nodes2Shader.Compilation.MathGraph;

namespace ShaderGraphToy.Representation.GraphNodes.GraphNodeComponents
{
    public interface INodeComponentView
    {
        string GetContent();
        void SetContent(string content);
        NodeEntry? GetData();
    }
}
