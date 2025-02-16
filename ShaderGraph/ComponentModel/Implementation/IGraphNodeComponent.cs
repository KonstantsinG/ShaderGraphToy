using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderGraph.ComponentModel.Implementation
{
    public interface IGraphNodeComponent
    {
        public enum ComponentType
        {
            Inscription,
            Input,
            List,
            Vector,
            Matrix,
            Color
        }
    }
}
