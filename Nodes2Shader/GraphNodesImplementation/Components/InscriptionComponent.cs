namespace Nodes2Shader.GraphNodesImplementation.Components
{
    public class InscriptionComponent : INodeComponent
    {
        public required string Title { get; set; }
        public string DefaultInput { get; set; } = "Ignore";
        public List<string> Formatting { get; set; } = [];
        public required bool HasInput { get; set; }
        public required bool HasOutput { get; set; }
        public required string InputType { get; set; }
        public required string OutputType { get; set; }
    }
}
