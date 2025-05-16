namespace ShaderGraphToy.Utilities.Serializers
{
    internal class NodeModel
    {
        public required int Id { get; set; }
        public required int TypeId { get; set; }
        public required double TranslateX { get; set; }
        public required double TranslateY { get; set; }
        public required List<string> Contents { get; set; } = [];
    }
}
