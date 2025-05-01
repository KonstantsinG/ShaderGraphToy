using Nodes2Shader.Compilation.MathGraph;
using Nodes2Shader.DataTypes;
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
            string blabla = ConstructMainFunctionBody(visualGraph);

            sb.AppendLine("}");
            
            result = CompilationResult.Success;
            return sb.ToString();
        }

        private static string ConstructMainFunctionBody(GraphData graph)
        {
            StringBuilder sb = new(), expSb;
            int constCtr = 0, inCtr = 0, outCtr = 0, opCtr = 0, funcCtr = 0;
            List<GraphNodeExpression> exps = [];
            GraphNodeExpression? ex;

            foreach (NodeData nd in graph.Nodes)
            {
                if (nd.GetInputs().Any(i => !i.DataRevealed && i.Value == "Error"))
                    throw new ArgumentException($"Node {nd.Id} have required fields with null values");

                // load node expression
                ex = exps.FirstOrDefault(x => x.TypeId == nd.Id);
                if (ex == null)
                {
                    ex = GraphNodeExpressionsSerializer.DeserializeExpression(nd.TypeId);
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
                int nodeOutputs = nd.GetUsedOutputsCount();
                for (int i = nodeOutputs - 1; i >= 0; i--)
                {
                    // cast input types to one of the available combinations
                    DataTypesConverter.RevealTypes(nd, ex.GetInputVariants());

                    expSb = new();
                    nd.Expressions.Add(ex.GetVariant(nodeVariant, i, nd.GetInputType())!);
                    expSb.AppendLine(nd.Expressions.Last().Expression);

                    // replace predefined constants
                    FillExpression(expSb, nd, i);
                    sb.AppendLine(expSb.ToString());

                    // set input data for all nodes connected to this
                    PropagateOutputsToInputs(graph, nd);
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

        private static void PropagateOutputsToInputs(GraphData graph, NodeData nd)
        {
            List<NodeEntry> outputs = nd.GetOutputs();
            List<NodeEntry> inputs;
            string name;

            for (int i = 0; i < outputs.Count; i++)
            {
                name = nd.GetName(i);
                inputs = FindConnectedInputs(graph, nd, i);

                foreach (NodeEntry inp in inputs)
                {
                    inp.Value = name;
                    inp.Type = outputs[i].Type;
                    inp.DataRevealed = true;
                }
            }
        }

        private static List<NodeEntry> FindConnectedInputs(GraphData graph, NodeData node, int outId)
        {
            List<NodeEntry> inputs = [];
            int ownId = node.Id, secId, secOutId;

            foreach (NodesConnection nc in node.OutputConnections)
            {
                if (nc.FirstNodeId == ownId)
                {
                    if (nc.FirstNodeConnectorId != outId) continue;

                    secId = nc.SecondNodeId;
                    secOutId = nc.SecondNodeConnectorId;
                }
                else
                {
                    if (nc.SecondNodeConnectorId != outId) continue;

                    secId = nc.FirstNodeId;
                    secOutId = nc.FirstNodeConnectorId;
                }

                inputs.Add(graph.GetEntry(secId, secOutId)!);
            }

            return inputs;
        }
    }
}
