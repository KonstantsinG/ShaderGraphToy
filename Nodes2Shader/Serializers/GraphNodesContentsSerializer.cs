using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Nodes2Shader.GraphNodesImplementation.Components;
using Nodes2Shader.GraphNodesImplementation.Contents;
using Nodes2Shader.Resources;
using Nodes2Shader.DataTypes;
using System.Reflection;

namespace Nodes2Shader.Serializers
{
    public static class GraphNodesContentsSerializer
    {
        public static List<GraphNodeContent> DeserializeAll()
        {
            var settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                Converters = [ new NodeComponentConverter() ]
            };

            List<GraphNodeContent> data = [];
            GraphNodesContentsContainer? container = null;
            String? json= null;

            foreach (string id in ResourceManager.GraphNodesContentsIds)
            {
                json = ResourceManager.GetGrahNodesContentsResource(id);
                container = JsonConvert.DeserializeObject<GraphNodesContentsContainer>(json, settings)!;
                data.AddRange(ConvertToDomainModel(container!));
            }

            return data;
        }

        public static GraphNodeContent? Deserialize(uint typeId)
        {
            var json = ResourceManager.GetGrahNodesContentsResource(typeId.ToString()[0].ToString());

            using (var stringReader = new StringReader(json))
            using (var jsonReader = new JsonTextReader(stringReader))
            {
                while (jsonReader.Read())
                {
                    if (jsonReader.TokenType == JsonToken.StartArray && jsonReader.Path == "GraphNodesContents")
                    {
                        while (jsonReader.Read() && jsonReader.TokenType != JsonToken.EndArray)
                        {
                            if (jsonReader.TokenType == JsonToken.StartObject)
                            {
                                var obj = JObject.Load(jsonReader);
                                if (obj["TypeId"]?.Value<uint>() == typeId)
                                {
                                    return ParseSingleContent(obj);
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }



        private static GraphNodeContent ParseSingleContent(JObject contentObj)
        {
            var settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                Converters = [ new NodeComponentConverter() ]
            };

            var intermediateContent = JsonConvert.DeserializeObject<IntermediateGraphNodeContent>(contentObj.ToString(), settings);

            return new GraphNodeContent
            {
                Id = intermediateContent!.TypeId,
                HasInput = intermediateContent.HasInput,
                HasOutput = intermediateContent.HasOutput,
                InputType = intermediateContent.InputType != null ? CreateDataTypeInstance(intermediateContent.InputType) : null,
                OutputType = intermediateContent.OutputType != null ? CreateDataTypeInstance(intermediateContent.OutputType) : null,
                Components = intermediateContent.Components
            };
        }

        private static List<GraphNodeContent> ConvertToDomainModel(GraphNodesContentsContainer container)
        {
            var result = new List<GraphNodeContent>();

            foreach (var content in container.GraphNodesContents)
            {
                var nodeContent = new GraphNodeContent
                {
                    Id = content.TypeId,
                    HasInput = content.HasInput,
                    HasOutput = content.HasOutput,
                    InputType = content.InputType != null ? CreateDataTypeInstance(content.InputType) : null,
                    OutputType = content.OutputType != null ? CreateDataTypeInstance(content.OutputType) : null,
                    Components = content.Components
                };

                result.Add(nodeContent);
            }

            return result;
        }

        private static DataType CreateDataTypeInstance(string typeName)
        {
            var type = Assembly.GetAssembly(typeof(DataType))?
                .GetTypes()
                .FirstOrDefault(t => t.Name == typeName && t.IsSubclassOf(typeof(DataType)));

            return (DataType)Activator.CreateInstance(type!)! ?? throw new JsonSerializationException($"Unknown data type: {typeName}"); ;
        }

        private class NodeComponentConverter : JsonConverter<INodeComponent>
        {
            public override INodeComponent ReadJson(JsonReader reader, Type objectType, INodeComponent? existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                JObject jo = JObject.Load(reader);
                string type = jo["Type"]!.Value<string>()!;

                INodeComponent component = type switch
                {
                    "Input" => new InputComponent
                    {
                        Title = jo["Title"]!.Value<string>()!,
                        IsReadonly = jo["IsReadonly"]!.Value<bool>(),
                        HasInput = jo["HasInput"]!.Value<bool>(),
                        InputType = jo["InputType"]?.Value<string>() != null ?
                            CreateDataTypeInstance(jo["InputType"]!.Value<string>()!) : null
                    },
                    "Vector" => new VectorComponent
                    {
                        Title = jo["Title"]!.Value<string>()!,
                        Contents = jo["Contents"]?.ToObject<List<string>>() ?? [],
                        IsReadonly = jo["IsReadonly"]!.Value<bool>(),
                        IsControlable = jo["IsControlable"]!.Value<bool>()
                    },
                    "Matrix" => new MatrixComponent
                    {
                        Title = jo["Title"]!.Value<string>()!,
                        Contents = jo["Contents"]?.ToObject<List<List<string>>>() ?? [],
                        IsReadonly = jo["IsReadonly"]!.Value<bool>(),
                        IsControlable = jo["IsControlable"]!.Value<bool>()
                    },
                    "List" => new ListComponent
                    {
                        Title = jo["Title"]!.Value<string>()!,
                        Contents = jo["Contents"]?.ToObject<List<string>>() ?? []
                    },
                    "Color" => new ColorComponent
                    {
                        Title = jo["Title"]!.Value<string>()!,
                        Content = jo["Content"]?.Value<string>() ?? string.Empty
                    },
                    "Inscription" => new InscriptionComponent
                    {
                        Title = jo["Title"]!.Value<string>()!,
                        HasInput = jo["HasInput"]!.Value<bool>(),
                        HasOutput = jo["HasOutput"]!.Value<bool>(),
                        InputType = jo["InputType"]?.Value<string>() != null ?
                            CreateDataTypeInstance(jo["InputType"]!.Value<string>()!) : null,
                        OutputType = jo["OutputType"]?.Value<string>() != null ?
                            CreateDataTypeInstance(jo["OutputType"]!.Value<string>()!) : null
                    },
                    _ => throw new JsonSerializationException($"Unknown component type: {type}")
                };

                if (component is InputComponent input && jo["Content"] != null)
                    input.Content = jo["Content"]!.Value<string>()!;

                if (component is InscriptionComponent inscription && jo["Formatting"] != null)
                    inscription.Formatting = jo["Formatting"]!.ToObject<List<string>>()!;

                return component;
            }

            public override void WriteJson(JsonWriter writer, INodeComponent? value, JsonSerializer serializer)
            {
                return;
            }
        }



        private class GraphNodesContentsContainer
        {
            public List<IntermediateGraphNodeContent> GraphNodesContents { get; set; } = [];
        }

        private class IntermediateGraphNodeContent
        {
            public required uint TypeId { get; set; }
            public required bool HasInput { get; set; }
            public required bool HasOutput { get; set; }
            public required string? InputType { get; set; }
            public required string? OutputType { get; set; }
            public required List<INodeComponent> Components { get; set; }
        }
    }
}