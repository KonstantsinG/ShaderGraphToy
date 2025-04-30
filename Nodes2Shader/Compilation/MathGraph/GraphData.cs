namespace Nodes2Shader.Compilation.MathGraph
{
    public class GraphData
    {
        private List<NodeData> _nodes = [];
        public List<NodeData> Nodes
        {
            get => _nodes;
            set
            {
                _nodes = value;
                _nodes.Sort((n1, n2) => n1.Layer.CompareTo(n2.Layer));
            }
        }

        public List<NodeData> GetLayer(int layer) => Nodes.Where(n => n.Layer == layer).ToList();
        public List<NodeData> GetEndpoints() => Nodes.Where(n => n.InputConnections.Count == 0).ToList();
    }
}
