namespace Nodes2Shader.Compilation.MathGraph
{
    public class NodeEntry
    {
        public string Type { get; set; } = string.Empty;
        public int Id { get; set; }
        public string Value { get; set; } = string.Empty;
        public bool DataRevealed { get; set; }

        public NodeEntry(string type, string value)
        {
            Type = type;
            Value = value;
            DataRevealed = true;
        }

        public NodeEntry()
        {
            DataRevealed = false;
        }
    }
}
