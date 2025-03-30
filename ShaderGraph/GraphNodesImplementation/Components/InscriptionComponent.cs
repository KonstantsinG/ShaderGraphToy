using ShaderGraph.DataTypes;

namespace ShaderGraph.GraphNodesImplementation.Components
{
    public class InscriptionComponent : INodeComponent
    {
        public required string Title { get; set; }
        public List<string> Formatting { get; set; } = [];
        public required bool HasInput { get; set; }
        public required bool HasOutput { get; set; }
        public required DataType? InputType { get; set; }
        public required DataType? OutputType { get; set; }
    }
}
