using ShaderGraph.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderGraph.ComponentModel.Implementation
{
    public class InscriptionComponent : IGraphNodeComponent
    {
        public required string Title { get; set; }
        public required bool HasInput { get; set; }
        public required bool HasOutput { get; set; }
        public required VariantConverter.DataType InputType { get; set; }
        public required VariantConverter.DataType OutputType { get; set; }
    }
}
