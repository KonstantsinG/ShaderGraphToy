using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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

namespace GUI.Controls
{
    /// <summary>
    /// Логика взаимодействия для NodesConnector.xaml
    /// </summary>
    public partial class NodesConnector : UserControl
    {
        private bool _isBusy = false;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (value) ellipse.Fill = (SolidColorBrush)FindResource(NodeColor);
                else ellipse.Fill = (SolidColorBrush)FindResource("Gray_02");

                _isBusy = value;
            }
        }

        public bool IsInput { get; set; }
        public int NodeId { get; set; }
        public int ConnectorId { get; set; }
        public string NodeColor { get; set; } = string.Empty;


        public NodesConnector()
        {
            InitializeComponent();
        }


        public Point GetGlobalCenter(FrameworkElement el)
        {
            Point localCenter = new(ActualWidth / 2, ActualHeight / 2);
            GeneralTransform transform = TransformToVisual(el);

            return transform.Transform(localCenter);
        }


        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!IsBusy) ellipse.Fill = (SolidColorBrush)FindResource("HighlightBlue");
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!IsBusy) ellipse.Fill = (SolidColorBrush)FindResource("Gray_02");
        }
    }
}
