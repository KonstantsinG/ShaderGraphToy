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
        public bool Selected { get; private set; }

        public event MouseButtonEventHandler HeaderPressed = delegate { };
        public event MouseEventHandler ConnectorPressed = delegate { };


        public GraphNodeBase()
        {
            InitializeComponent();

            inputConnector.IsInput = true;
            outputConnector.IsInput = false;

            GraphNodeBaseVM vm = new();
            DataContext = vm;
            vm.GetOwnConnectors = GetConnectors;
            vm.RaiseConnectorPressedEvent = RaiseConnectorPressedEvent;
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

        public List<NodesConnector> GetConnectors()
        {
            List<NodesConnector> conns = [];

            if (inputConnector.Visibility == Visibility.Visible) conns.Add(inputConnector);
            if (outputConnector.Visibility == Visibility.Visible) conns.Add(outputConnector);

            return conns;
        }

        public void RaiseConnectorPressedEvent(object sender, MouseEventArgs e)
        {
            ConnectorPressed.Invoke(sender, e);
        }
    }
}
