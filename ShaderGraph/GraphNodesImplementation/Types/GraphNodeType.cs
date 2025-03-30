namespace ShaderGraph.GraphNodesImplementation.Types
{
    public class GraphNodeType : INodeInfo
    {
        public required uint Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required List<string> Synonyms { get; set; }
        public required string Color { get; set; }
        public required List<OperationType> OperationsTypes { get; set; }

        public bool UsingOperations
        {
            get => OperationsTypes.Count > 1;
        }

        public bool UsingSubOperations
        {
            get
            {
                if (OperationsTypes.Count > 0)
                    return OperationsTypes[0].OperationsSubTypes.Count > 0;
                else
                    return false;
            }
        }
    }
}
