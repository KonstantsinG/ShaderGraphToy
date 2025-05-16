namespace ShaderGraphToy.Utilities.Serializers
{
    internal class GraphModel
    {
        public required List<NodeModel> Nodes { get; set; } = [];
        public required List<ConnectionModel> Connections { get; set; } = [];
    }
}
