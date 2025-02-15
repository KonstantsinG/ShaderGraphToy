using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ShaderGraph.ComponentModel
{
    public class GraphNodeOperationInfo
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required List<GraphNodeSubOperationInfo> SubTypes { get; set; }
    }
}
