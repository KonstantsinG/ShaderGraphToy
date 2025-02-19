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
