namespace Nodes2Shader.GraphNodesImplementation.Expressions
{
    public class GraphNodeExpression
    {
        public required int TypeId {  get; set; }
        public required string Name { get; set; }
        public required List<ExpressionVariant> ExpressionVariants { get; set; }


        public ExpressionVariant? GetVariant(int variant, int output, string inputTypes)
        {
            foreach (ExpressionVariant v in ExpressionVariants)
            {
                if (v.Variant == variant && 
                    v.Output == output && 
                   (v.InputTypes.Any(it => it == inputTypes) || v.InputTypes.Contains(string.Empty) || v.InputTypes.Count == 0))
                    return v;
            }

            return ExpressionVariants.FirstOrDefault();
        }

        public List<string> GetInputVariants()
        {
            List<string> vars = [];

            foreach (ExpressionVariant v in ExpressionVariants)
                vars.AddRange(v.InputTypes);

            vars = vars.Distinct().ToList();
            if (vars.All(string.IsNullOrWhiteSpace))
                return [];

            return vars;
        }
    }
}
