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
            nameof(Model), typeof(ListComponent), typeof(ListComponent), new PropertyMetadata(null));

        public ListComponent Model
        {
            get { return (ListComponent)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }


        public ListComponent()
        {
            InitializeComponent();
        }
    }
}
