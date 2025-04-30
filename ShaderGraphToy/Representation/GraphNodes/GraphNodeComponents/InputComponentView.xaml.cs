using Nodes2Shader.Compilation.MathGraph;
using Nodes2Shader.GraphNodesImplementation.Components;
using System.Windows;
using System.Windows.Controls;

namespace ShaderGraphToy.Representation.GraphNodes.GraphNodeComponents
{
    /// <summary>
    /// Логика взаимодействия для InputComponent.xaml
    /// </summary>
    public partial class InputComponentView : UserControl, INodeComponentView
    {
        public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(
            nameof(Model), typeof(InputComponent), typeof(InputComponentView), new PropertyMetadata(null));

        public InputComponent Model
        {
            get => (InputComponent)GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }

        public InputComponentView()
        {
            InitializeComponent();

            inputConnector.IsInput = true;
        }

        public NodesConnector? GetConnector()
        {
            if (Model.HasInput) return inputConnector;
            else return null;
        }

        public NodeEntry GetData()
        {
            NodeEntry.EntryType type;
            int id = -1;
            
            if (Model.HasInput)
            {
                type = NodeEntry.EntryType.Input;
                id = GetConnector()!.ConnectorId;
            }
            else
            {
                type = NodeEntry.EntryType.Value;
            }

            return new("Float", Model.Content, type) { Id = id };
        }
    }
}
