namespace ShaderGraphToy.Utilities.Serializers
{
    public class ConnectionModel
    {
        public required int InputNode { get; set; }
        public required int OutputNode { get; set; }
        public required int InputConnector { get; set; }
        public required int OutputConnector { get; set; }


        public static bool operator!=(ConnectionModel left, ConnectionModel right)
        {
            return !(left == right);
        }
        public static bool operator==(ConnectionModel left, ConnectionModel right)
        {
            return (left.InputNode == right.InputNode && left.OutputNode == right.OutputNode && 
                    left.InputConnector == right.InputConnector && left.OutputConnector == right.OutputConnector);
        }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
