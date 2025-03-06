using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShaderGraph.ComponentModel.Info;
using ShaderGraph.Converters;
using ShaderGraph.ComponentModel.Implementation;

namespace ShaderGraph.Assemblers
{
    public class GraphNodesAssembler
    {
        private readonly string _graphNodesTypesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JsonData/graphNodesTypes.json");
        private readonly string _graphNodesTypesContentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JsonData/graphNodesTypesContent.json");

        public static readonly GraphNodesAssembler Instance = new();

        private GraphNodesAssembler()
        {
        }


        public GraphNodeTypeInfo? GetTypeInfo(string type)
        {
            string jsonContent = File.ReadAllText(_graphNodesTypesPath);
            JObject jObject = JObject.Parse(jsonContent);
            JArray? graphNodesTypesArray = jObject["GraphNodesTypes"] as JArray;

            foreach (var node in graphNodesTypesArray!)
            {
                if (node["Name"]?.ToString() == type)
                    return node.ToObject<GraphNodeTypeInfo>()!;
            }

            return null;
        }

        public GraphNodeTypeInfo? GetTypeInfo(int id)
        {
            string jsonContent = File.ReadAllText(_graphNodesTypesPath);
            JObject jObject = JObject.Parse(jsonContent);
            JArray? graphNodesTypesArray = jObject["GraphNodesTypes"] as JArray;

            foreach (var node in graphNodesTypesArray!)
            {
                if (node["TypeId"]?.ToString() == id.ToString())
                    return node.ToObject<GraphNodeTypeInfo>()!;
            }

            return null;
        }

        public List<GraphNodeTypeInfo> GetTypesInfo()
        {
            List<GraphNodeTypeInfo> infos = [];
            string jsonContent = File.ReadAllText(_graphNodesTypesPath);
            JObject jObject = JObject.Parse(jsonContent);
            JArray? graphNodesTypesArray = jObject["GraphNodesTypes"] as JArray;

            foreach (var node in graphNodesTypesArray!)
                infos.Add(node.ToObject<GraphNodeTypeInfo>()!);

            return infos;
        }

        public GraphNodeTypeContentInfo? GetTypeContentInfo(int id)
        {
            var settings = new JsonSerializerSettings
            {
                Converters = { new ComponentTypesConverter() },
                MissingMemberHandling = MissingMemberHandling.Error
            };

            string jsonContent = File.ReadAllText(_graphNodesTypesContentPath);
            var jObject = JObject.Parse(jsonContent);
            var graphNodesTypesArray = jObject["GraphNodesTypesContent"] as JArray;

            foreach (var node in graphNodesTypesArray ?? [])
            {
                if (node["TypeId"]?.ToString() == id.ToString())
                {
                    return node.ToObject<GraphNodeTypeContentInfo>(JsonSerializer.CreateDefault(settings));
                }
            }

            return null;
        }
    }
}
