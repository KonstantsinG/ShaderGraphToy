using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Nodes2Shader.GraphNodesImplementation.Expressions;
using Nodes2Shader.Resources;

namespace Nodes2Shader.Serializers
{
    public static class GraphNodeExpressionsSerializer
    {
        public static GraphNodeExpression DeserializeExpression(int typeId)
        {
            string json = ResourceManager.GetGraphNodesExpressionsResource(typeId.ToString()[0].ToString());

            using (var stringReader = new StringReader(json))
            using (var jsonReader = new JsonTextReader(stringReader))
            {
                while (jsonReader.Read())
                {
                    if (jsonReader.TokenType == JsonToken.StartArray && jsonReader.Path == "GraphNodesExpressions")
                    {
                        while (jsonReader.Read() && jsonReader.TokenType != JsonToken.EndArray)
                        {
                            if (jsonReader.TokenType == JsonToken.StartObject)
                            {
                                var obj = JObject.Load(jsonReader);
                                if (obj["TypeId"]?.Value<int>() == typeId)
                                {
                                    return ParseExpression(obj);
                                }
                            }
                        }
                    }
                }
            }

            throw new KeyNotFoundException($"Expression with TypeId {typeId} not found in JSON");
        }

        private static GraphNodeExpression ParseExpression(JObject expressionObj)
        {
            var expression = new GraphNodeExpression
            {
                TypeId = expressionObj["TypeId"]?.Value<int>() ?? throw new JsonException("Missing required TypeId"),
                Name = expressionObj["Name"]?.Value<string>() ?? throw new JsonException("Missing required Name"),
                ExpressionVariants = []
            };

            if (expressionObj["ExpressionVariants"] is JArray variantsArray)
            {
                foreach (var variantToken in variantsArray)
                {
                    if (variantToken is JObject variantObj)
                    {
                        var variant = new ExpressionVariant
                        {
                            Variant = variantObj["Variant"]?.Value<int>() ?? throw new JsonException("Missing required Variant"),
                            Output = variantObj["Output"]?.Value<int>() ?? throw new JsonException("Missing required Output"),
                            InputTypes = variantObj["InputTypes"]?.ToObject<List<string>>() ?? [],
                            Expression = variantObj["Expression"]?.Value<string>() ?? string.Empty,
                            ExternalFunctions = variantObj["ExternalFunctions"]?.ToObject<List<string>>() ?? []
                        };
                        expression.ExpressionVariants.Add(variant);
                    }
                }
            }

            return expression;
        }



        public static ExternalFunction DeserializeExternalFunction(string path)
        {
            string json = ResourceManager.GetExternalFunctionsResource();

            using (var stringReader = new StringReader(json))
            using (var jsonReader = new JsonTextReader(stringReader))
            {
                while (jsonReader.Read())
                {
                    if (jsonReader.TokenType == JsonToken.StartArray && jsonReader.Path == "ExternalFunctions")
                    {
                        while (jsonReader.Read() && jsonReader.TokenType != JsonToken.EndArray)
                        {
                            if (jsonReader.TokenType == JsonToken.StartObject)
                            {
                                var obj = JObject.Load(jsonReader);
                                if (obj["Path"]?.Value<string>() == path)
                                {
                                    return ParseExternalFunction(obj);
                                }
                            }
                        }
                    }
                }
            }

            throw new KeyNotFoundException($"External function with path '{path}' not found in JSON");
        }

        private static ExternalFunction ParseExternalFunction(JObject functionObj)
        {
            var body = functionObj["Body"] is JArray bodyArray
                ? string.Join(Environment.NewLine, bodyArray.Select(j => j.Value<string>()))
                : string.Empty;

            return new ExternalFunction
            {
                Type = functionObj["Type"]?.Value<string>() ?? throw new JsonException("Missing required Type"),
                Path = functionObj["Path"]?.Value<string>() ?? throw new JsonException("Missing required Path"),
                Body = body
            };
        }

        public static List<ExternalFunction> DeserializeAllExternalFunctions()
        {
            string json = ResourceManager.GetExternalFunctionsResource();

            var settings = new JsonSerializerSettings
            {
                Converters = [ new ExternalFunctionConverter() ]
            };

            var container = JsonConvert.DeserializeObject<ExternalFunctionsContainer>(json, settings);
            return container?.ExternalFunctions ?? [];
        }




        private class ExternalFunctionsContainer
        {
            public required List<ExternalFunction> ExternalFunctions { get; set; }
        }

        private class ExternalFunctionConverter : JsonConverter<ExternalFunction>
        {
            public override ExternalFunction ReadJson(JsonReader reader, Type objectType, ExternalFunction? existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var obj = JObject.Load(reader);
                return ParseExternalFunction(obj);
            }

            public override void WriteJson(JsonWriter writer, ExternalFunction? value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}
