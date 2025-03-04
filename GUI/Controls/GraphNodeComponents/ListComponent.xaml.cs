using ShaderGraph.ComponentModel.Implementation.NodeComponents;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Controls.GraphNodeComponents
{
    /// <summary>
    /// Логика взаимодействия для ListComponent.xaml
    /// </summary>
    public partial class ListComponent : UserControl
    {
        public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(
            nameof(Model), typeof(ListComponentData), typeof(ListComponent), new PropertyMetadata(null));

        public ListComponentData Model
        {
            get { return (ListComponentData)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }


        public ListComponent()
        {
            InitializeComponent();
        }
    }
}
