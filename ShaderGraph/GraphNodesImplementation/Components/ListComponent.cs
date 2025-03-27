namespace ShaderGraph.GraphNodesImplementation.Components
{
    public class ListComponent : INodeComponent
    {
        public required string Title { get; set; }
        public required List<string> Contents { get; set; }
    }
}
