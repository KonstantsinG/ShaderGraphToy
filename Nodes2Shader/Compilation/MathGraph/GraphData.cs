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
                _nodes.Sort((n1, n2) => n2.Layer.CompareTo(n1.Layer));
            }
        }

        public List<NodeData> GetLayer(int layer) => Nodes.Where(n => n.Layer == layer).ToList();
        public List<NodeData> GetEndpoints() => Nodes.Where(n => n.InputConnections.Count == 0).ToList();

        public NodeData GetOutput() => Nodes.First(n => n.TypeId == 3);

        public NodeEntry? GetEntry(int nodeId, int entryId)
        {
            foreach (NodeData node in Nodes)
            {
                if (node.Id == nodeId)
                {
                    foreach (NodeEntry entry in node.Entries)
                    {
                        if (entry.Id == entryId) return entry;
                    }
                }
            }

            return null;
        }
    }
}
