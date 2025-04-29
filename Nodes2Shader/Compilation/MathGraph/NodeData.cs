namespace Nodes2Shader.Compilation.MathGraph
{
    public class NodeData (int id)
    {
        public int Id { get; set; } = id;
        public List<NodesConnection> InputConnections { get; set; } = [];
        public List<NodesConnection> OutputConnections { get; set; } = [];
        public List<NodeEntry> Entries { get; set; } = [];
        public NodeEntry? NodeInput { get; set; } = null;
        public NodeEntry? NodeOutput { get; set; } = null;
        public int Layer { get; set; }
    }
}
