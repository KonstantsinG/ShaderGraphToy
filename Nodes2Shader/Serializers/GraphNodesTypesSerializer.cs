using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nodes2Shader.GraphNodesImplementation.Types;
using Nodes2Shader.Resources;

namespace Nodes2Shader.Serializers
{
    public static class GraphNodesTypesSerializer
    {
        public static List<GraphNodeType> DeserializeAll()
        {
            if (CacheManager.GraphNodeTypesAvailable)
                return CacheManager.GraphNodeTypes;

            var settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };

            var json = ResourceManager.GetGrahNodesTypesInfoResource();
            var intermediateData = JsonConvert.DeserializeObject<GraphNodesTypesContainer>(json, settings);

            var types = ConvertToDomainModel(intermediateData!);
            CacheManager.Cache(types);

            return types;
        }

        public static GraphNodeType? Deserialize(uint typeId)
        {
            if (CacheManager.GraphNodeTypesAvailable)
                return CacheManager.GraphNodeTypes.FirstOrDefault(t => t.Id == typeId);

            var json = ResourceManager.GetGrahNodesTypesInfoResource();

            using var stringReader = new StringReader(json);
            using var jsonReader = new JsonTextReader(stringReader);
            while (jsonReader.Read())
            {
                if (jsonReader.TokenType == JsonToken.StartArray && jsonReader.Path == "GraphNodesTypes")
                {
                    while (jsonReader.Read() && jsonReader.TokenType != JsonToken.EndArray)
                    {
                        if (jsonReader.TokenType == JsonToken.StartObject)
                        {
                            var obj = JObject.Load(jsonReader);
                            if (obj["TypeId"]?.Value<uint>() == typeId)
                            {
                                return ParseSingleNodeType(obj);
                            }
                        }
                    }
                }
            }

            return null;
        }



        private static GraphNodeType ParseSingleNodeType(JObject nodeObj)
        {
            var intermediateNode = new IntermediateGraphNodeType
            {
                Name = nodeObj["Name"]?.Value<string>() ?? string.Empty,
                Description = nodeObj["Description"]?.Value<string>() ?? string.Empty,
                Synonyms = nodeObj["Synonyms"]?.ToObject<List<string>>() ?? [],
                TypeId = nodeObj["TypeId"]?.Value<uint>() ?? 0,
                Color = nodeObj["Color"]?.Value<string>() ?? string.Empty,
                OperationsTypes = []
            };

            if (nodeObj["OperationsTypes"] is JArray operationsArray)
            {
                foreach (var opToken in operationsArray)
                {
                    if (opToken is JObject opObj)
                    {
                        var operation = new IntermediateOperationType
                        {
                            Name = opObj["Name"]?.Value<string>() ?? string.Empty,
                            Description = opObj["Description"]?.Value<string>() ?? string.Empty,
                            Synonyms = opObj["Synonyms"]?.ToObject<List<string>>() ?? [],
                            TypeId = opObj["TypeId"]?.Value<uint>() ?? 0,
                            OperationsSubTypes = []
                        };

                        if (opObj["OperationsSubTypes"] is JArray subTypesArray)
                        {
                            foreach (var subTypeToken in subTypesArray)
                            {
                                if (subTypeToken is JObject subTypeObj)
                                {
                                    operation.OperationsSubTypes.Add(new IntermediateOperationSubType
                                    {
                                        Name = subTypeObj["Name"]?.Value<string>() ?? string.Empty,
                                        Description = subTypeObj["Description"]?.Value<string>() ?? string.Empty,
                                        Synonyms = subTypeObj["Synonyms"]?.ToObject<List<string>>() ?? [],
                                        TypeId = subTypeObj["TypeId"]?.Value<uint>() ?? 0
                                    });
                                }
                            }
                        }

                        intermediateNode.OperationsTypes.Add(operation);
                    }
                }
            }

            return ConvertToDomainModel(new GraphNodesTypesContainer {GraphNodesTypes = [ intermediateNode ]}).FirstOrDefault()!;
        }

        private static List<GraphNodeType> ConvertToDomainModel(GraphNodesTypesContainer container)
        {
            var result = new List<GraphNodeType>();

            foreach (var nodeType in container.GraphNodesTypes)
            {
                var graphNodeType = new GraphNodeType
                {
                    Id = nodeType.TypeId,
                    Name = nodeType.Name,
                    Description = nodeType.Description,
                    Synonyms = nodeType.Synonyms,
                    Color = nodeType.Color,
                    OperationsTypes = []
                };

                if (nodeType.OperationsTypes != null)
                {
                    foreach (var operationType in nodeType.OperationsTypes)
                    {
                        var operation = new OperationType
                        {
                            Id = operationType.TypeId,
                            Name = operationType.Name,
                            Description = operationType.Description,
                            Synonyms = operationType.Synonyms,
                            OperationsSubTypes = []
                        };

                        if (operationType.OperationsSubTypes != null)
                        {
                            foreach (var subType in operationType.OperationsSubTypes)
                            {
                                operation.OperationsSubTypes.Add(new OperationSubType
                                {
                                    Id = subType.TypeId,
                                    Name = subType.Name,
                                    Description = subType.Description,
                                    Synonyms = subType.Synonyms
                                });
                            }
                        }

                        graphNodeType.OperationsTypes.Add(operation);
                    }
                }

                result.Add(graphNodeType);
            }

            return result;
        }



        private class GraphNodesTypesContainer
        {
            public List<IntermediateGraphNodeType> GraphNodesTypes { get; set; } = [];
        }

        private class IntermediateGraphNodeType
        {
            public required string Name { get; set; }
            public required string Description { get; set; }
            public required List<string> Synonyms { get; set; }
            public required uint TypeId { get; set; }
            public required string Color { get; set; }
            public required List<IntermediateOperationType> OperationsTypes { get; set; }
        }

        private class IntermediateOperationType
        {
            public required string Name { get; set; }
            public required string Description { get; set; }
            public required List<string> Synonyms { get; set; }
            public required uint TypeId { get; set; }
            public List<IntermediateOperationSubType> OperationsSubTypes { get; set; } = [];
        }

        private class IntermediateOperationSubType
        {
            public required string Name { get; set; }
            public required string Description { get; set; }
            public required List<string> Synonyms { get; set; }
            public required uint TypeId { get; set; }
        }
    }
}
