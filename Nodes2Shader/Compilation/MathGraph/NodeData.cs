using Nodes2Shader.GraphNodesImplementation.Expressions;
using System.Xml;

namespace Nodes2Shader.Compilation.MathGraph
{
    public class NodeData (int id)
    {
        public int Id { get; set; } = id;
        public List<NodesConnection> InputConnections { get; set; } = [];
        public List<NodesConnection> OutputConnections { get; set; } = [];
        public List<NodeEntry> Entries { get; set; } = [];
        public NodeEntry? NodeInput { get; set; } = null;
        public NodeEntry? NodeOutput { get; set; } = null;
        public int Layer { get; set; }


        public List<ExpressionVariant> Expressions { get; set; } = [];

        public string VarName { get; set; }
        public int VarId { get; set; }


        public int GetVariant()
        {
            foreach (NodeEntry e in Entries)
            {
                if (e.Behavior == NodeEntry.EntryType.Variant)
                    return int.Parse(e.Value);
            }

            return 0;
        }

        public int GetUsaedOutputsCount()
        {
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

            return outs.Count;
        }

        public string GetName(int outId) => VarName.Replace("<idx>", VarId.ToString()).Replace("<outIdx>", outId.ToString());

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
    }
}
