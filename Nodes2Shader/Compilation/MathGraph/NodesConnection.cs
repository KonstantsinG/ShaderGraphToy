namespace Nodes2Shader.Compilation.MathGraph
{
    public class NodesConnection (int nId1, int nId2, int cId1, int cId2)
    {
        public int FirstNodeId { get; set; } = nId1;
        public int SecondNodeId { get; set; } = nId2;
        public int FirstNodeConnectorId { get; set; } = cId1;
        public int SecondNodeConnectorId { get; set; } = cId2;
    }
}
