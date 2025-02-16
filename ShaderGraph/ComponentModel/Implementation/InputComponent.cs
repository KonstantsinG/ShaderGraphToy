using ShaderGraph.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderGraph.ComponentModel.Implementation
{
    public class InputComponent : IGraphNodeComponent
    {
        public required string Title { get; set; }
        public required string Content { get; set; }
        public required bool IsReadonly {  get; set; }
        public required bool HasInput { get; set; }
        public required VariantConverter.DataType InputType { get; set; }
    }
}
