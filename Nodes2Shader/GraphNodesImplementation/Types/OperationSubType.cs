namespace Nodes2Shader.GraphNodesImplementation.Types
{
    public class OperationSubType : INodeInfo
    {
        public required uint Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required List<string> Synonyms { get; set; }
    }
}
