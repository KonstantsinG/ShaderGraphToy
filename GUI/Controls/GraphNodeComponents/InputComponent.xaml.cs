using ShaderGraph.ComponentModel.Implementation.NodeComponents;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Controls.GraphNodeComponents
{
    /// <summary>
    /// Логика взаимодействия для InputComponent.xaml
    /// </summary>
    public partial class InputComponent : UserControl
    {
        public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(
            nameof(Model), typeof(InputComponentData), typeof(InputComponent), new PropertyMetadata(null));

        public InputComponentData Model
        {
            get => (InputComponentData)GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }

        public InputComponent()
        {
            InitializeComponent();
        }
    }
}
