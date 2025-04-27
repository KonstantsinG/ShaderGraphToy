using Nodes2Shader.Compilation.MathGraph;
using Nodes2Shader.GraphNodesImplementation.Components;
using System.Windows;
using System.Windows.Controls;

namespace ShaderGraphToy.Representation.GraphNodes.GraphNodeComponents
{
    /// <summary>
    /// Логика взаимодействия для ListComponent.xaml
    /// </summary>
    public partial class ListComponentView : UserControl, INodeComponentView
    {
        public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(
            nameof(Model), typeof(ListComponent), typeof(ListComponentView), new PropertyMetadata(null));

        public ListComponent Model
        {
            get { return (ListComponent)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }


        public ListComponentView()
        {
            InitializeComponent();
        }


        public NodeEntry GetData()
        {
            return new("Int", cBox.SelectedIndex.ToString());
        }
    }
}
