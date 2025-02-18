using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderGraph.ComponentModel.Implementation
{
    public class MatrixComponentData : IGraphNodeComponent
    {
        public required string Title { get; set; }
        public required List<string> Contents { get; set; }
        public required bool IsControlable { get; set; }
        public required bool IsReadonly { get; set; }
    }
}
