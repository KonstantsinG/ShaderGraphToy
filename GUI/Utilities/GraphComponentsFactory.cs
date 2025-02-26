using GUI.Controls;
using GUI.Controls.GraphNodeComponents;
using ShaderGraph.Assemblers;
using ShaderGraph.ComponentModel.Implementation;
using ShaderGraph.ComponentModel.Info;
using ShaderGraph.ComponentModel.Info.Wrappers;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GUI.Utilities
{
    public static class GraphComponentsFactory
    {
        public static UserControl ConstructComponent(IGraphNodeComponent data)
        {
            return data switch
            {
                InscriptionComponentData inscData => new InscriptionComponent() { Model = inscData },
                InputComponentData inpData => new InputComponent() { Model = inpData },
                VectorComponentData vecData => new VectorComponent() { Model = vecData },
                MatrixComponentData matData => new MatrixComponent() { Model = matData },
                ListComponentData lstData => new ListComponent() { Model = lstData },
                ColorComponentData colorData => new ColorComponent() { Model = colorData },
                _ => throw new ArgumentException("Unsupported component type")
            };
        }

        public static List<UserControl> ConstructComponents(List<IGraphNodeComponent> components)
        {
            List<UserControl> controls = [];
            foreach (IGraphNodeComponent comp in components)
                controls.Add(ConstructComponent(comp));

            return controls;
        }

        public static IEnumerable<TreeViewerItem> GetNodeTypesInfo()
        {
            IEnumerable<TreeViewerItem> types = [];
            List<GraphNodeTypeInfo> data = GraphNodesAssembler.Instance.GetTypesInfo();
            TreeViewerItem? currType;
            TreeViewerItem? currSubType;
            TreeViewerItem? currSubSubType;

            foreach (GraphNodeTypeInfo type in data)
            {
                currType = new TreeViewerItem() { Model = type };

                if (type.UsingOperations)
                {
                    foreach (GraphNodeOperationInfo subType in type.OperationsTypes)
                    {
                        currSubType = new TreeViewerItem() { Model = subType };
                        currType.Children.Add(currSubType);

                        if (type.UsingSubOperations)
                        {
                            foreach (GraphNodeSubOperationInfo subSubType in subType.SubTypes)
                            {
                                currSubSubType = new TreeViewerItem() { Model = subSubType };
                                currSubType.Children.Add(currSubSubType);
                            }
                        }
                    }
                }

                types = types.Append(currType);
            }

            return types;
        }
    }
}
