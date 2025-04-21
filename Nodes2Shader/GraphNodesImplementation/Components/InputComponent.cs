using Nodes2Shader.DataTypes;

namespace Nodes2Shader.GraphNodesImplementation.Components
{
    public class InputComponent : INodeComponent
    {
        public required string Title { get; set; }
        public required bool IsReadonly { get; set; }
        public string Content { get; set; } = string.Empty;
        public required bool HasInput { get; set; }
        public required string InputType { get; set; }
    }
}
