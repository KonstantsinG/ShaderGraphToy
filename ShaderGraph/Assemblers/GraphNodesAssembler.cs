using Newtonsoft.Json.Linq;
using ShaderGraph.ComponentModel.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderGraph.Assemblers
{
    public class GraphNodesAssembler
    {
        private readonly string _graphNodesTypesPath = "JsonData/graphNodesTypes.json";

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
    }
}
