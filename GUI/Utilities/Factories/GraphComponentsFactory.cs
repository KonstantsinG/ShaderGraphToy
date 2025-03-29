using GUI.Representation.GraphNodes.GraphNodeComponents;
using ShaderGraph.GraphNodesImplementation.Components;
using System.Windows.Controls;

namespace GUI.Utilities
{
    public static class GraphComponentsFactory
    {
        public static UserControl ConstructComponent(INodeComponent data)
        {
            return data switch
            {
                //InscriptionComponentData inscData => new InscriptionComponent() { Model = inscData },
                //InputComponentData inpData => new InputComponent() { Model = inpData },
                //VectorComponentData vecData => new VectorComponent() { Model = vecData },
                //MatrixComponentData matData => new MatrixComponent() { Model = matData },
                //ListComponentData lstData => new ListComponent() { Model = lstData },
                //ColorComponentData colorData => new ColorComponent() { Model = colorData },
                //_ => throw new ArgumentException("Unsupported component type")
            };
        }

        public static List<UserControl> ConstructComponents(List<INodeComponent> components)
        {
            List<UserControl> controls = [];
            foreach (INodeComponent comp in components)
                controls.Add(ConstructComponent(comp));

            return controls;
        }

        //public static IEnumerable<TreeViewerItem> GetNodeTypesInfo()
        //{
        //    IEnumerable<TreeViewerItem> types = [];
        //    List<GraphNodeTypeInfo> data = GraphNodesAssembler.Instance.GetTypesInfo();
        //    TreeViewerItem? currType;
        //    TreeViewerItem? currSubType;
        //    TreeViewerItem? currSubSubType;

        //    foreach (GraphNodeTypeInfo type in data)
        //    {
        //        currType = new TreeViewerItem() { Model = type };

        //        if (type.UsingOperations)
        //        {
        //            foreach (GraphNodeOperationInfo subType in type.OperationsTypes)
        //            {
        //                currSubType = new TreeViewerItem() { Model = subType };
        //                currType.Children.Add(currSubType);

        //                if (type.UsingSubOperations)
        //                {
        //                    foreach (GraphNodeSubOperationInfo subSubType in subType.SubTypes)
        //                    {
        //                        currSubSubType = new TreeViewerItem() { Model = subSubType };
        //                        currSubType.Children.Add(currSubSubType);
        //                    }
        //                }
        //            }
        //        }

        //        types = types.Append(currType);
        //    }

        //    return types;
        //}
    }
}
