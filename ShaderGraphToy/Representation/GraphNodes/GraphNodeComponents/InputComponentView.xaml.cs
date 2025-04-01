using Nodes2Shader.GraphNodesImplementation.Components;
using System.Windows;
using System.Windows.Controls;

namespace ShaderGraphToy.Representation.GraphNodes.GraphNodeComponents
{
    /// <summary>
    /// Логика взаимодействия для InputComponent.xaml
    /// </summary>
    public partial class InputComponentView : UserControl
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
    }
}
