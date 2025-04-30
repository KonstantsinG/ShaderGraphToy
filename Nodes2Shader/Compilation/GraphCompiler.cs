using Nodes2Shader.Compilation.MathGraph;
using Nodes2Shader.GraphNodesImplementation.Expressions;
using Nodes2Shader.Serializers;
using System.Text;

namespace Nodes2Shader.Compilation
{
    public enum CompilationResult
    {
        Success,
        SomeGlslError
    }

    public static class GraphCompiler
    {
        public static string Compile(GraphData visualGraph, out CompilationResult result)
        {
            StringBuilder sb = new(); StringBuilder sb2 = new();
            sb.AppendLine(GraphNodeExpressionsSerializer.DeserializeExternalFunction("header").Body);
            
            // external functions

            sb.AppendLine(GraphNodeExpressionsSerializer.DeserializeExternalFunction("entryp").Body);

            // main function body

            sb.AppendLine("}");
            
            result = CompilationResult.Success;
            return sb.ToString();
        }

        private static string ConstructMainFunction(GraphData graph)
        {
            StringBuilder sb = new(), expSb;
            int constCtr = 0, inCtr = 0, outCtr = 0, opCtr = 0, funcCtr = 0;
            List<GraphNodeExpression> exps = [];
            GraphNodeExpression? ex;

            foreach (NodeData nd in graph.Nodes)
            {
                // load node expression
                ex = ex = exps.FirstOrDefault(x => x.TypeId == nd.Id);
                if (ex == null)
                {
                    ex = GraphNodeExpressionsSerializer.DeserializeExpression(nd.Id);
                    exps.Add(ex);
                }

                nd.VarId = ex.TypeId.ToString()[0] switch
                {
                    '1' => constCtr++,
                    '2' => inCtr++,
                    '3' => outCtr++,
                    '4' => opCtr++,
                    '5' => funcCtr++,
                    _ => 0
                };
                nd.VarName = ex.Name;

                // reveal node variant
                int nodeVariant = nd.GetVariant();

                // generate code for each node output
                int nodeOutputs = nd.GetUsaedOutputsCount();
                for (int i = nodeOutputs; i >= 0; i--)
                {
                    // cast input types to one of the available combinations

                    // 1. get own output types
                    // 2. try to cast it to one of the available

                    // cast all inputs to a one common type (chose greatest from given)
                    // if no input types configuration was given

                    expSb = new();
                    nd.Expressions.Add(ex.GetVariant(nodeVariant, i, "")!);
                    expSb.AppendLine(nd.Expressions.Last().Expression);

                    // replace predefined constants
                    FillExpression(expSb, nd, i);
                }
            }

            return sb.ToString();
        }

        private static void FillExpression(StringBuilder sb, NodeData nd, int outId)
        {
            sb.Replace("<idx>", nd.VarId.ToString());
            sb.Replace("<outIdx>", outId.ToString());
            sb.Replace("<name>", nd.GetName(outId));

            List<NodeEntry> inputs = nd.GetInputs();
            List<NodeEntry> outputs = nd.GetOutputs();

            for (int i = 0; i < inputs.Count; i++)
            {
                sb.Replace($"<inType{i + 1}>", inputs[i].Type);
                sb.Replace($"<val{i + 1}>", inputs[i].Value);
            }

            for (int i = 0; i < outputs.Count; i++)
            {
                sb.Replace($"<outType{i + 1}>", outputs[i].Type);
                outputs[i].Value = nd.GetName(i);
            }
        }
    }
}
