using ShaderGraph.ComponentModel.Implementation.NodeComponents;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Controls.GraphNodeComponents
{
    /// <summary>
    /// Логика взаимодействия для ColorComponent.xaml
    /// </summary>
    public partial class ColorComponent : UserControl
    {
        public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(
            nameof(Model), typeof(ColorComponentData), typeof(ColorComponent), new PropertyMetadata(null));

        public ColorComponentData Model
        {
            get { return (ColorComponentData)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }


        public ColorComponent()
        {
            InitializeComponent();
        }
    }
}
