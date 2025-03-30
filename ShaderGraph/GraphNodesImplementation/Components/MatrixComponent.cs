namespace ShaderGraph.GraphNodesImplementation.Components
{
    public class MatrixComponent : INodeComponent
    {
        public required string Title { get; set; }
        public required List<List<string>> Contents { get; set; }
        public required bool IsReadonly { get; set; }
        public required bool IsControlable { get; set; }
    }
}
