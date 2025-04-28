namespace Nodes2Shader.Compilation.MathGraph
{
    public class NodeData (int id)
    {
        public string InputType { get; set; } = string.Empty;
        public string OutputType { get; set; } = string.Empty;
        public int Id { get; set; } = id;
        public List<NodesConnection> Connections { get; set; } = [];
        public List<NodeEntry> Entries { get; set; } = [];
        public int Layer { get; set; }
    }
}
