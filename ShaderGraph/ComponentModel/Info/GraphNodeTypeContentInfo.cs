using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ShaderGraph.ComponentModel.Implementation;
using ShaderGraph.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderGraph.ComponentModel.Info
{
    public class GraphNodeTypeContentInfo
    {
        public required int TypeId {  get; set; }
        public required string Name { get; set; }
        public required bool HasInput {  get; set; }
        public required bool HasOutput { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public required VariantConverter.DataType? InputType { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public required VariantConverter.DataType? OutputType { get; set; }

        public required List<IGraphNodeComponent> Components { get; set; }
    }
}
