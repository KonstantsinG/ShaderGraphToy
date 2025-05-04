using Nodes2Shader.GraphNodesImplementation.Expressions;
using System.Text;

namespace Nodes2Shader.Compilation.MathGraph
{
    public class NodeData (int id)
    {
        public int Id { get; set; } = id;
        public int TypeId {  get; set; }
        public List<NodesConnection> InputConnections { get; set; } = [];
        public List<NodesConnection> OutputConnections { get; set; } = [];
        public List<NodeEntry> Entries { get; set; } = [];
        public NodeEntry? NodeInput { get; set; } = null;
        public NodeEntry? NodeOutput { get; set; } = null;
        public int Layer { get; set; }


        public ExpressionVariant? Expression { get; set; } = null;
        public string VarName { get; set; } = string.Empty;
        public int VarId { get; set; }
        public string VarInput { get; set; } = string.Empty;


        public int GetVariant()
        {
            foreach (NodeEntry e in Entries)
            {
                if (e.Behavior == NodeEntry.EntryType.Variant)
                    return int.Parse(e.Value);
            }

            return 0;
        }

        public string GetInputTypes()
        {
            // input nodes have virtual input (because they are startpoints)
            if (TypeId.ToString()[0] == '2')
                return NodeOutput!.Type;

            StringBuilder sb = new();
            bool isFirst = true;
            int Idcounter = 0;

            foreach (NodeEntry e in GetInputs())
            {
                if (!isFirst) sb.Append(',');
                isFirst = false;

                // add null for each excluded input
                while (Idcounter < e.Id)
                {
                    sb.Append("null,");
                    Idcounter++;
                }
                Idcounter++;

                sb.Append(e.Type);
            }

            return sb.ToString();
        }

        public List<int> GetOutputsIds()
        {
            // output node have virtual output (because it endpoint)
            if (TypeId == 3) return [0];

            List<int> outs = [];

            foreach (NodesConnection con in OutputConnections)
            {
                if (con.FirstNodeId == Id)
                {
                    if (!outs.Contains(con.FirstNodeConnectorId))
                        outs.Add(con.FirstNodeConnectorId);
                }
                else
                {
                    if (!outs.Contains(con.SecondNodeConnectorId))
                        outs.Add(con.SecondNodeConnectorId);
                }
            }

            return outs;
        }

        public string GetName(int outId) => VarName.Replace("<idx>", VarId.ToString()).Replace("<outIdx>", outId.ToString());

        public void SetOutputValueAndData(int outId)
        {
            List<NodeEntry> outputs = GetOutputs();
            if (outputs.Count == 0) return;

            NodeEntry output = outputs.Where(o => o.Id == outId).First();
            output.Value = GetName(outId);
            output.Type = Expression!.OutputType;

            if (output.Type.Contains('<')) // expression may have preprocessors like <inType1>
            {
                List<NodeEntry> inputs = GetInputs();
                int id = int.Parse(output.Type[7].ToString()) - 1;
                output.Type = inputs[id].Type;
            }
        }

        public List<NodeEntry> GetInputs()
        {
            List<NodeEntry> entrs = [];
            if (NodeInput != null) entrs.Add(NodeInput);

            foreach (NodeEntry e in Entries)
            {
                if (e.Behavior == NodeEntry.EntryType.Input || e.Behavior == NodeEntry.EntryType.Value)
                    entrs.Add(e);
            }

            return entrs;
        }

        public List<NodeEntry> GetOutputs()
        {
            List<NodeEntry> entrs = [];
            if (NodeOutput != null) entrs.Add(NodeOutput);

            foreach (NodeEntry e in Entries)
            {
                if (e.Behavior == NodeEntry.EntryType.Output)
                    entrs.Add(e);
            }

            return entrs;
        }

        public List<NodeEntry> GetAllEntries()
        {
            List<NodeEntry> entries = new(Entries);
            if (NodeInput != null) entries.Add(NodeInput);
            if (NodeOutput != null) entries.Add(NodeOutput);

            return entries;
        }
    }
}
