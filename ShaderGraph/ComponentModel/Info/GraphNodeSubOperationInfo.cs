using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderGraph.ComponentModel.Info
{
    public class GraphNodeSubOperationInfo
    {
        public required int TypeId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required List<string> Synonyms { get; set; }
    }
}
