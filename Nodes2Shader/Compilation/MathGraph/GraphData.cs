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


        public NodeEntry? GetEntry(int nodeId, int entryId, NodeEntry.EntryType entryType)
        {
            foreach (NodeData node in Nodes)
            {
                if (node.Id == nodeId)
                {
                    foreach (NodeEntry entry in node.GetAllEntries())
                    {
                        if (entry.Id == entryId && entry.Behavior == entryType)
                            return entry;
                    }
                }
            }

            return null;
        }
    }
}
