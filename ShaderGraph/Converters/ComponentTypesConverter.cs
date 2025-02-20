using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ShaderGraph.ComponentModel.Implementation;
using System.Diagnostics.CodeAnalysis;

namespace ShaderGraph.Converters
{
    public class ComponentTypesConverter : JsonConverter<IGraphNodeComponent>
    {
        public override IGraphNodeComponent ReadJson(JsonReader reader, Type objectType, IGraphNodeComponent? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            var type = obj["Type"]?.ToString().Trim();

            return type switch
            {
                "Inscription" => CreateComponent<InscriptionComponentData>(obj, serializer),
                "Input" => CreateComponent<InputComponentData>(obj, serializer),
                "Vector" => CreateComponent<VectorComponentData>(obj, serializer),
                "Matrix" => CreateComponent<MatrixComponentData>(obj, serializer),
                "List" => CreateComponent<ListComponentData>(obj, serializer),
                "Color" => CreateComponent<ColorComponentData>(obj, serializer),
                _ => throw new JsonSerializationException($"Unknown component type: {type}")
            };
        }

        private static T CreateComponent<T>(JObject obj, JsonSerializer serializer) where T : IGraphNodeComponent
        {
            var component = Activator.CreateInstance<T>();
            serializer.Populate(obj.CreateReader(), component);
            return component;
        }

        public override void WriteJson(JsonWriter writer, IGraphNodeComponent? value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
