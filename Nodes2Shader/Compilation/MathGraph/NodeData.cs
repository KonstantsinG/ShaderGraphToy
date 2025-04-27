namespace Nodes2Shader.Compilation.MathGraph
{
    public class NodeData (int id)
    {
        public int Id { get; set; } = id;
        public List<int> Connections { get; set; } = [];
        public List<NodeEntry> Entries { get; set; } = [];
        public uint Layer { get; set; }
    }
}
