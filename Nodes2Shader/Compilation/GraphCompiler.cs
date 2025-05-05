using Nodes2Shader.Compilation.MathGraph;
using Nodes2Shader.DataTypes;
using Nodes2Shader.GraphNodesImplementation.Expressions;
using Nodes2Shader.Serializers;
using System.Diagnostics;
using System.Text;

namespace Nodes2Shader.Compilation
{
    public static class GraphCompiler
    {
        public static string Compile(GraphData visualGraph)
        {
            StringBuilder sb = new();

            string main = ConstructMainFunctionBody(visualGraph); // main function body
            string extf = ConstructExternalFunctions(visualGraph); // external functions

            sb.AppendLine(GraphNodeExpressionsSerializer.DeserializeExternalFunction("header").Body);
            sb.AppendLine(); sb.Append(extf);
            sb.AppendLine(GraphNodeExpressionsSerializer.DeserializeExternalFunction("entryp").Body);
            sb.Append(main);
            sb.AppendLine("}");
            
            return sb.ToString();
        }

        private static string ConstructExternalFunctions(GraphData graph)
        {
            StringBuilder constSb = new();
            StringBuilder uniformSb = new();
            StringBuilder funcSb = new();
            List<ExternalFunction> loaded = [];
            ExternalFunction? f;

            foreach (NodeData nd in graph.Nodes)
            {
                foreach (string extf in nd.Expression!.ExternalFunctions)
                {
                    f = loaded.FirstOrDefault(l => l.Path == extf);

                    if (f == null)
                    {
                        f = GraphNodeExpressionsSerializer.DeserializeExternalFunction(extf);
                        loaded.Add(f);
                    }
                    
                    if (f.Type == "defconst") constSb.AppendLine(f.Body);
                    else if (f.Type == "uniform") uniformSb.AppendLine(f.Body);
                    else if (f.Type == "function") funcSb.AppendLine(f.Body);
                    // for now, preprocessors will be ignored
                }
            }

            if (constSb.Length > 0) constSb.AppendLine();
            if (uniformSb.Length > 0) constSb.AppendLine(uniformSb.ToString());
            if (funcSb.Length > 0) constSb.AppendLine(funcSb.ToString());

            return constSb.ToString();
        }

        private static string ConstructMainFunctionBody(GraphData graph)
        {
            StringBuilder sb = new(), expSb;
            int constCtr = 0, inCtr = 0, outCtr = 0, opCtr = 0, funcCtr = 0;
            List<GraphNodeExpression> exps = [];
            GraphNodeExpression? ex;
            string matchingInput;
            int lastLayer = graph.Nodes[0].Layer;

            foreach (NodeData nd in graph.Nodes)
            {
                // load node expression
                ex = exps.FirstOrDefault(x => x.TypeId == nd.TypeId);
                if (ex == null)
                {
                    ex = GraphNodeExpressionsSerializer.DeserializeExpression(nd.TypeId);
                    exps.Add(ex);
                }

                // set code variable id and name
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

                // get node variant (value from list component if there is)
                int nodeVariant = nd.GetVariant();

                // generate code for each node output
                List<int> nodeOutputs = nd.GetOutputsIds();
                foreach (int i in nodeOutputs)
                {
                    expSb = new();
                    if (lastLayer != nd.Layer) expSb.AppendLine();
                    lastLayer = nd.Layer;

                    // find matching expression variant
                    nd.Expression = ex.FindMatchingExpressionVariant(nodeVariant, i, nd.GetInputTypes(), out matchingInput);
                    nd.VarInput = matchingInput;

                    if (nd.Expression.Expression.Length > 0)
                    {
                        expSb.Append('\t');
                        expSb.Append(nd.Expression.Expression);
                    }

                    // cast all node input values to correct type
                    DataTypesConverter.CastInputs(nd);
                    // set revealed output value and type
                    nd.SetOutputValueAndData(i);

                    // replace preprocessors
                    FillExpression(expSb, nd, i);
                    if (expSb.Length > 0) sb.AppendLine(expSb.ToString());

                    // set input data for all nodes connected to it
                    PropagateOutputToInputs(graph, nd);
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
            }
        }

        private static void PropagateOutputToInputs(GraphData graph, NodeData nd)
        {
            List<NodeEntry> outputs = nd.GetOutputs();
            List<NodeEntry> inputs;
            string name;

            for (int i = 0; i < outputs.Count; i++)
            {
                int outId = outputs[i].Id;
                name = nd.GetName(outId);
                inputs = FindConnectedInputs(graph, nd, outId);
                if (inputs.Count == 0) throw new InvalidOperationException("No inputs connected to this output!");

                foreach (NodeEntry inp in inputs)
                {
                    inp.Value = name;
                    inp.Type = outputs[i].Type;
                }
            }
        }

        private static List<NodeEntry> FindConnectedInputs(GraphData graph, NodeData node, int outId)
        {
            List<NodeEntry> inputs = [];
            NodeEntry? entry;
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

                entry = graph.GetEntry(secId, secOutId, NodeEntry.EntryType.Input);
                if (entry != null) inputs.Add(entry);
                else throw new InvalidOperationException($"NodeEntry ({secId}, {secOutId}) is not found.");
            }

            return inputs;
        }
    }
}
