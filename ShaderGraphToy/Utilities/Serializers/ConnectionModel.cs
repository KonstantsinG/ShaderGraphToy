namespace ShaderGraphToy.Utilities.Serializers
{
    internal class ConnectionModel
    {
        public required int InputNode { get; set; }
        public required int OutputNode { get; set; }
        public required int InputConnector { get; set; }
        public required int OutputConnector { get; set; }
    }
}
