using Nodes2Shader.DataTypes;
using Nodes2Shader.GraphNodesImplementation.Components;

namespace Nodes2Shader.GraphNodesImplementation.Contents
{
    public class GraphNodeContent
    {
        public required uint Id { get; set; }
        public required bool HasInput { get; set; }
        public required bool HasOutput { get; set; }
        public required DataType? InputType { get; set; }
        public required DataType? OutputType { get; set; }
        public required List<INodeComponent> Components { get; set; }
    }
}
