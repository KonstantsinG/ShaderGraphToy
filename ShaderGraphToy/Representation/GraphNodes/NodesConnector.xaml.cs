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

namespace ShaderGraphToy.Representation.GraphNodes
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

        public int ConnectionsCount { get; set; } = 0;
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

        public void Disconnect()
        {
            ConnectionsCount--;
            
            if (IsInput || (!IsInput && ConnectionsCount == 0))
                IsBusy = false;
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
