namespace ShaderGraph.GraphNodesImplementation.Components
{
    public class VectorComponent : INodeComponent
    {
        public required string Title { get; set; }
        public required List<string> Contents { get; set; }
        public required bool IsReadonly { get; set; }
        public required bool IsControlable { get; set; }
    }
}
