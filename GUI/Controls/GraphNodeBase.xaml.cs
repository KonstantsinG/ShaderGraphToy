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
        private bool _isTaken = false;
        private Point _mouseOffset;


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
            borderRect.Fill = (Brush)FindResource("HighlightBlue");
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            borderRect.Fill = (Brush)FindResource("Gray_00");
        }

        private void HeaderPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isTaken = true;
            _mouseOffset = e.GetPosition(this);
            headerPanel.CaptureMouse();
        }

        private void HeaderPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isTaken = false;
            headerPanel.ReleaseMouseCapture();
        }

        private void HeaderPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isTaken)
            {
                Point currentPosition;

                if (Parent is Canvas)
                {
                    currentPosition = e.GetPosition(Parent as Canvas);

                    Canvas.SetLeft(this, currentPosition.X - _mouseOffset.X);
                    Canvas.SetTop(this, currentPosition.Y - _mouseOffset.Y);
                }
                else
                {
                    if (((UserControl)Parent).Parent is Canvas)
                    {
                        currentPosition = e.GetPosition(((UserControl)Parent).Parent as Canvas);

                        Canvas.SetLeft((UserControl)Parent, currentPosition.X - _mouseOffset.X);
                        Canvas.SetTop((UserControl)Parent, currentPosition.Y - _mouseOffset.Y);
                    }
                }
            }
        }
    }
}
