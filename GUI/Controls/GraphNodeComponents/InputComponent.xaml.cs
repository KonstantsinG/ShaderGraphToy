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
            nameof(Model), typeof(InputComponent), typeof(InputComponent), new PropertyMetadata(null));

        public InputComponent Model
        {
            get => (InputComponent)GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }

        public InputComponent()
        {
            InitializeComponent();

            inputConnector.IsInput = true;
        }

        public List<NodesConnector> GetConnectors()
        {
            //if (Model.HasInput) return [inputConnector];
            //else return [];
            return [];
        }
    }
}
