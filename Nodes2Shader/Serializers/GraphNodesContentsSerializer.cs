﻿using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Nodes2Shader.GraphNodesImplementation.Components;
using Nodes2Shader.GraphNodesImplementation.Contents;
using Nodes2Shader.Resources;

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
            GraphNodesContentsContainer? container;
            String? json;

            foreach (string id in ResourceManager.GraphNodesTypesIds)
            {
                json = ResourceManager.GetGraphNodesContentsResource(id);
                container = JsonConvert.DeserializeObject<GraphNodesContentsContainer>(json, settings)!;
                data.AddRange(ConvertToDomainModel(container!));
            }

            return data;
        }

        public static GraphNodeContent? Deserialize(uint typeId)
        {
            var json = ResourceManager.GetGraphNodesContentsResource(typeId.ToString()[0].ToString());

            using var stringReader = new StringReader(json);
            using var jsonReader = new JsonTextReader(stringReader);
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
                InputType = intermediateContent.InputType,
                OutputType = intermediateContent.OutputType,
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
                    InputType = content.InputType,
                    OutputType = content.OutputType,
                    Components = content.Components
                };

                result.Add(nodeContent);
            }

            return result;
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
                        InputType = (jo["InputType"]?.Value<string>()) ?? string.Empty
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
                        InputType = (jo["InputType"]?.Value<string>()) ?? string.Empty,
                        OutputType = (jo["OutputType"]?.Value<string>()) ?? string.Empty
                    },
                    "Texture" => new TextureComponent(),

                    _ => throw new JsonSerializationException($"Unknown component type: {type}")
                };

                if (component is InputComponent input && jo["Content"] != null)
                    input.Content = jo["Content"]!.Value<string>()!;

                if (component is InscriptionComponent inscription)
                {
                    if (jo["Formatting"] != null)
                        inscription.Formatting = jo["Formatting"]!.ToObject<List<string>>()!;
                    if (jo["DefaultInput"] != null)
                        inscription.DefaultInput = jo["DefaultInput"]!.ToObject<string>()!;
                }

                if (component is TextureComponent texture)
                {
                    if (jo["ImageData"] != null)
                        texture.ImageData = jo["ImageData"]!.ToObject<byte[]>()!;
                    if (jo["WrapS"] != null)
                        texture.WrapS = jo["WrapS"]!.ToObject<int>()!;
                    if (jo["WrapT"] != null)
                        texture.WrapT = jo["WrapT"]!.ToObject<int>()!;
                    if (jo["FilterMag"] != null)
                        texture.FilterMag = jo["FilterMag"]!.ToObject<int>()!;
                    if (jo["FilterMin"] != null)
                        texture.FilterMin = jo["FilterMin"]!.ToObject<int>()!;
                }

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
            public required string InputType { get; set; }
            public required string OutputType { get; set; }
            public required List<INodeComponent> Components { get; set; }
        }
    }
}