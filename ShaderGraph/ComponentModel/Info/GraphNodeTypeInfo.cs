using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderGraph.ComponentModel.Info
{
    public class GraphNodeTypeInfo
    {
        public required string Name { get; set; }
        public required string Color { get; set; }
        public required List<GraphNodeOperationInfo> OperationsTypes { get; set; }
    }
}
