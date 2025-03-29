using ShaderGraph.GraphNodesImplementation.Types;

namespace GUI.Representation.GraphNodes.Wrappers
{
    public class TreeViewerNodeInfo(INodeInfo info) : INodeInfo
    {
        public uint Id { get; set; } = info.Id;
        public string Name { get; set; } = info.Name;
        public string Description { get; set; } = info.Description;
        public List<string> Synonyms { get; set; } = info.Synonyms;

        public bool IsExpanded { get; set; }
        public bool IsVisible { get; set; }
        public List<TreeViewerNodeInfo> Children { get; set; } = [];
    }
}
