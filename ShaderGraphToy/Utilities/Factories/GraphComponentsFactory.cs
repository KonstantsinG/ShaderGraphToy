using ShaderGraphToy.Representation.GraphNodes.GraphNodeComponents;
using Nodes2Shader.GraphNodesImplementation.Components;
using System.Windows.Controls;

namespace ShaderGraphToy.Utilities
{
    public static class GraphComponentsFactory
    {
        public static INodeComponentView ConstructComponent(INodeComponent data)
        {
            return data switch
            {
                InscriptionComponent inscData => new InscriptionComponentView() { Model = inscData },
                InputComponent inpData => new InputComponentView() { Model = inpData },
                VectorComponent vecData => new VectorComponentView() { Model = vecData },
                MatrixComponent matData => new MatrixComponentView() { Model = matData },
                ListComponent lstData => new ListComponentView() { Model = lstData },
                ColorComponent colorData => new ColorComponentView() { Model = colorData },
                _ => throw new ArgumentException("Unsupported component type")
            };
        }

        public static List<INodeComponentView> ConstructComponents(List<INodeComponent> components)
        {
            List<INodeComponentView> controls = [];

            foreach (INodeComponent comp in components)
                controls.Add(ConstructComponent(comp));

            return controls;
        }
    }
}
