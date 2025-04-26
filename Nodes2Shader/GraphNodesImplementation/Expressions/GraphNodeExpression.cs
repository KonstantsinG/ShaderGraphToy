namespace Nodes2Shader.GraphNodesImplementation.Expressions
{
    public class GraphNodeExpression
    {
        public required int TypeId {  get; set; }
        public required string Name { get; set; }
        public required List<ExpressionVariant> ExpressionVariants { get; set; }
    }
}
