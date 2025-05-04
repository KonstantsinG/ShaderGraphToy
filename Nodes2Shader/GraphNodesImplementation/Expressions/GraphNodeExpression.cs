using Nodes2Shader.DataTypes;

namespace Nodes2Shader.GraphNodesImplementation.Expressions
{
    public class GraphNodeExpression
    {
        public required int TypeId {  get; set; }
        public required string Name { get; set; }
        public required List<ExpressionVariant> ExpressionVariants { get; set; }


        public ExpressionVariant FindMatchingExpressionVariant(int variant, int output, string inputTypes, out string matchingInput)
        {
            string[] inputs1 = inputTypes.Split(','), inputs2;

            // first try to find matching types with strong typing
            foreach (ExpressionVariant expV in ExpressionVariants)
            {
                if (expV.Variant != variant || expV.Output != output) continue;

                // if input types is empty - does not matter wich type you have
                if (expV.InputTypes.Count == 0)
                {
                    matchingInput = "Greatest";
                    return expV;
                }

                foreach (string v in expV.InputTypes)
                {
                    inputs2 = v.Split(',');

                    // if node input type is too short - push null's in back
                    if (inputs1.Length < inputs2.Length)
                        inputs1 = MakeSameSizes(inputs1, inputs2);

                    if (DataTypesConverter.IsTypesRelevant(inputs1, inputs2))
                    {
                        matchingInput = v;
                        return expV;
                    }
                }
            }

            // if there is nothing - try weak typing
            foreach (ExpressionVariant expV in ExpressionVariants)
            {
                if (expV.Variant != variant || expV.Output != output) continue;

                foreach (string v in expV.InputTypes)
                {
                    inputs2 = v.Split(',');

                    // if node input type is too short - push null's in back
                    if (inputs1.Length < inputs2.Length)
                        inputs1 = MakeSameSizes(inputs1, inputs2);

                    if (DataTypesConverter.IsTypesRelevant(inputs1, inputs2, false))
                    {
                        matchingInput = v;
                        return expV;
                    }
                }
            }

            throw new InvalidOperationException("Operation does not support this combination of inputs.");
        }



        private static string[] MakeSameSizes(string[] inputs1, string[] inputs2)
        {
            string[] newInputs = new string[inputs2.Length];

            for (int i = 0; i < inputs1.Length; i++)
                newInputs[i] = inputs1[i];
            for (int i = inputs1.Length; i < inputs2.Length; i++)
                newInputs[i] = "null";

            return newInputs;
        }
    }
}
