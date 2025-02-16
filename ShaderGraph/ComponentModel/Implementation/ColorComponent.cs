using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderGraph.ComponentModel.Implementation
{
    public class ColorComponent : IGraphNodeComponent
    {
        public required string Title { get; set; }
        public required string Content { get; set; }
    }
}
