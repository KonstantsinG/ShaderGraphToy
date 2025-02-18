using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderGraph.ComponentModel.Implementation
{
    public class ColorComponentData : IGraphNodeComponent
    {
        public required string Title { get; set; }
        public required string Content { get; set; }
    }
}
