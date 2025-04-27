

namespace Nodes2Shader.Compilation.MathGraph
{
    public class GraphData
    {
        private List<NodeData> _nodes = [];
        public List<NodeData> Nodes
        {
            get => _nodes;
            private set => _nodes = value;
        }

        private List<int> _layers = [];
        public List<int> Layers
        {
            get => _layers;
            private set => _layers = value;
        }
    }
}
