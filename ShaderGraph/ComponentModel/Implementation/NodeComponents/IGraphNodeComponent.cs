namespace ShaderGraph.ComponentModel.Implementation.NodeComponents
{
    public interface IGraphNodeComponent
    {
        public enum ComponentType
        {
            Inscription,
            Input,
            List,
            Vector,
            Matrix,
            Color
        }

        public string Title { get; set; }
    }
}
