using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShaderGraph.ComponentModel.Info;
using ShaderGraph.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
