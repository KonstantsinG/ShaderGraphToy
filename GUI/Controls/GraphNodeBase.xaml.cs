using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GUI.Controls
{
    /// <summary>
    /// Логика взаимодействия для GraphNodeBase.xaml
    /// </summary>
    public partial class GraphNodeBase : UserControl
    {
        public int NodeId { get; set; }
        public bool Selected { get; private set; }

        public event MouseButtonEventHandler HeaderPressed = delegate { };


        public GraphNodeBase()
        {
            InitializeComponent();

            GraphNodeBaseVM vm = new();
            DataContext = vm;
            operationsCBox.SelectionChanged += vm.OperationsComboBox_SelectionChanged;
            subOperationsCBox.SelectionChanged += vm.SubOperationsComboBox_SelectionChanged;
        }


        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!Selected) borderRect.Fill = (Brush)FindResource("HighlightBlue");
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!Selected) borderRect.Fill = (Brush)FindResource("Gray_00");
        }

        public void ToggleSelection(bool isSelected)
        {
            Selected = isSelected;

            if (isSelected)
            {
                borderRect.Fill = (Brush)FindResource("SelectedBlue");
                borderRect.Margin = new Thickness(-3);
            }
            else
            {
                borderRect.Fill = (Brush)FindResource("Gray_00");
                borderRect.Margin = new Thickness(-1);
            }
        }

        private void HeaderPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HeaderPressed.Invoke(this, e);
        }
    }
}
