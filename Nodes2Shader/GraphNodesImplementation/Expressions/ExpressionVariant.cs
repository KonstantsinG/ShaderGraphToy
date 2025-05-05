namespace Nodes2Shader.GraphNodesImplementation.Expressions
{
    public class ExpressionVariant
    {
        public required int Variant {  get; set; }
        public required int Output { get; set; }
        public required string OutputType { get; set; }
        public required List<string> InputTypes { get; set; }
        public required string Expression { get; set; }
        public required List<string> ExternalFunctions { get; set; }
    }
}
