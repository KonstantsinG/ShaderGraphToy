using ShaderGraph.GraphNodesImplementation.Components;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Representation.GraphNodes.GraphNodeComponents
{
    /// <summary>
    /// Логика взаимодействия для ListComponent.xaml
    /// </summary>
    public partial class ListComponentView : UserControl
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
    }
}
