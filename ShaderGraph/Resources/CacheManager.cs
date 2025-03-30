using ShaderGraph.GraphNodesImplementation.Types;

namespace ShaderGraph.Resources
{
    internal static class CacheManager
    {
        public static List<GraphNodeType> GraphNodeTypes { get; set; } = [];

        public static bool GraphNodeTypesAvailable
        {
            get => GraphNodeTypes.Count > 0;
        }


        public static void Cache(List<GraphNodeType> types)
        {
            GraphNodeTypes = new(types);
        }

        public static void ClearGraphNodeTypes()
        {
            GraphNodeTypes.Clear();
        }
    }
}
