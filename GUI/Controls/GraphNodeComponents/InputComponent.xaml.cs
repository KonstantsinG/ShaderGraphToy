using ShaderGraph.ComponentModel.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
