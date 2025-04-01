using Nodes2Shader.GraphNodesImplementation.Components;
using System.Windows;
using System.Windows.Controls;

namespace ShaderGraphToy.Representation.GraphNodes.GraphNodeComponents
{
    /// <summary>
    /// Логика взаимодействия для ColorComponent.xaml
    /// </summary>
    public partial class ColorComponentView : UserControl
    {
        public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(
            nameof(Model), typeof(ColorComponent), typeof(ColorComponentView), new PropertyMetadata(null));

        public ColorComponent Model
        {
            get { return (ColorComponent)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }


        public ColorComponentView()
        {
            InitializeComponent();
        }
    }
}
