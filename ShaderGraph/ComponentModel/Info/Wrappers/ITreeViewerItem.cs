using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderGraph.ComponentModel.Info.Wrappers
{
    public interface ITreeViewerItem
    {
        public int TypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Synonyms { get; set; }
    }
}
