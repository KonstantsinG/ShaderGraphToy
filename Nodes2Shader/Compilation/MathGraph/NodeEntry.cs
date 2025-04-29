namespace Nodes2Shader.Compilation.MathGraph
{
    public class NodeEntry
    {
        public enum EntryType
        {
            Input, Output, Value
        }

        public EntryType Behavior { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public bool DataRevealed { get; set; }

        public NodeEntry(string type, string value, EntryType behavior)
        {
            Type = type;
            Value = value;
            Behavior = behavior;
            DataRevealed = true;
        }

        public NodeEntry()
        {
            DataRevealed = false;
        }
    }
}
