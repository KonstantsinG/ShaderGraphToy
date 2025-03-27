namespace ShaderGraph.GraphNodesImplementation.Types
{
    public class OperationType : INodeInfo
    {
        public required uint Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required List<string> Synonyms { get; set; }
        public List<OperationSubType> OperationsSubTypes { get; set; } = [];
    }
}
