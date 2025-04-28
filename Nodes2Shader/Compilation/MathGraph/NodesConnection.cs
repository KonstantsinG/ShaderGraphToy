namespace Nodes2Shader.Compilation.MathGraph
{
    public class NodesConnection (int nId1, int nId2, int cId1, int cId2)
    {
        public int FirstNodeId { get; set; } = nId1;
        public int SecondNodeId { get; set; } = nId2;
        public int FirstNodeConnectorId { get; set; } = cId1;
        public int SecondNodeConnectorId { get; set; } = cId2;


        public bool IsEqual(int nId1, int nId2, int cId1, int cId2)
        {
            return FirstNodeId == nId1 && SecondNodeId == nId2 && FirstNodeConnectorId == cId1 && SecondNodeConnectorId == cId2;
        }

        public bool IsPart(int nodeId, int conId)
        {
            return (FirstNodeId == nodeId && FirstNodeConnectorId == conId) || 
                   (SecondNodeId == nodeId && SecondNodeConnectorId == conId);
        }
    }
}
