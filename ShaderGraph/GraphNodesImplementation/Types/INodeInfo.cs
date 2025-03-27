namespace ShaderGraph.GraphNodesImplementation.Types
{
    public interface INodeInfo
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Synonyms { get; set; }
    }
}
