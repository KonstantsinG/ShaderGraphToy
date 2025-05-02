namespace Nodes2Shader.Compilation.MathGraph
{
    public class NodeEntry
    {
        public enum EntryType
        {
            Input, Output, Value, Variant
        }

        public EntryType Behavior { get; set; }
        public int Id { get; set; } = -1;
        public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;

        public NodeEntry(string type, string value, EntryType behavior)
        {
            Type = type;
            Value = value;
            Behavior = behavior;
        }

        public NodeEntry()
        {
        }
    }
}
