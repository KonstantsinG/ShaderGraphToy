using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ShaderGraph.ComponentModel.Implementation;
using System;
using System.Collections.Generic;
using System.Drawing;
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
                IGraphNodeComponent.ComponentType.Inscription => jsonObject.ToObject<InscriptionComponent>(serializer),
                IGraphNodeComponent.ComponentType.Input => jsonObject.ToObject<InputComponent>(serializer),
                IGraphNodeComponent.ComponentType.Vector => jsonObject.ToObject<VectorComponent>(serializer),
                IGraphNodeComponent.ComponentType.Matrix => jsonObject.ToObject<MatrixComponent>(serializer),
                IGraphNodeComponent.ComponentType.List => jsonObject.ToObject<ListComponent>(serializer),
                IGraphNodeComponent.ComponentType.Color => jsonObject.ToObject<ColorComponent>(serializer),
                _ => throw new NotSupportedException($"Unknown type: {type}"),
            };
        }

        public override void WriteJson(JsonWriter writer, IGraphNodeComponent? value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
