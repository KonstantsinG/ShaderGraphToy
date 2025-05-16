using Newtonsoft.Json;
using ShaderGraphToy.Representation.GraphNodes;
using System.IO;

namespace ShaderGraphToy.Utilities.Serializers
{
    internal static class GraphSerializer
    {
        private static readonly JsonSerializerSettings _settings = new()
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Error
        };


        public static void Serialize(List<GraphNodeBase> nodes, string path)
        {
            GraphModel graph;

            //string json = JsonConvert.SerializeObject(graph);
            //File.WriteAllText(path, json);
        }

        public static GraphModel DeserializeFromFile(string filePath)
        {

            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<GraphModel>(json, _settings)
                ?? throw new FileFormatException("Unable to load graph, source file is corrupted");
        }
    }
}
