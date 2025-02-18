using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ShaderGraph.ComponentModel.Implementation;
using System.Diagnostics.CodeAnalysis;

namespace ShaderGraph.Converters
{
    public class ComponentTypesConverter : JsonConverter<IGraphNodeComponent>
    {
        [return: MaybeNull]
        public override IGraphNodeComponent ReadJson(JsonReader reader, Type objectType, IGraphNodeComponent? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            IGraphNodeComponent.ComponentType type = (IGraphNodeComponent.ComponentType)Enum.Parse(typeof(IGraphNodeComponent.ComponentType), jsonObject["Type"]?.ToString() ?? "");

            return type switch
            {
                IGraphNodeComponent.ComponentType.Inscription => jsonObject.ToObject<InscriptionComponentData>(serializer),
                IGraphNodeComponent.ComponentType.Input => jsonObject.ToObject<InputComponentData>(serializer),
                IGraphNodeComponent.ComponentType.Vector => jsonObject.ToObject<VectorComponentData>(serializer),
                IGraphNodeComponent.ComponentType.Matrix => jsonObject.ToObject<MatrixComponentData>(serializer),
                IGraphNodeComponent.ComponentType.List => jsonObject.ToObject<ListComponentData>(serializer),
                IGraphNodeComponent.ComponentType.Color => jsonObject.ToObject<ColorComponentData>(serializer),
                _ => throw new NotSupportedException($"Unknown type: {type}"),
            };
        }

        public override void WriteJson(JsonWriter writer, IGraphNodeComponent? value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
