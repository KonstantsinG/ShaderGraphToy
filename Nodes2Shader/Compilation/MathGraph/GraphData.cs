

namespace Nodes2Shader.Compilation.MathGraph
{
    public class GraphData
    {
        public List<NodeData> Nodes { get; set; } = [];

        public List<NodeData> GetLayer(int layer) => Nodes.Where(n => n.Layer == layer).ToList();
        public List<NodeData> GetEndpoints() => Nodes.Where(n => n.InputConnections.Count == 0).ToList();
    }
}
