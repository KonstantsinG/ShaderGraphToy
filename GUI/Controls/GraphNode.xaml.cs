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

namespace GUI.Controls
{
    /// <summary>
    /// Логика взаимодействия для GraphNode.xaml
    /// </summary>
    public partial class GraphNode : UserControl
    {
        private bool isDragging = false;
        private Point mouseOffset;
        private bool isConnecting = false;

        public event EventHandler<ConnectionEventArgs> ConnectionStarted;
        public event EventHandler<ConnectionEventArgs> ConnectionEnded;

        public GraphNode()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!isConnecting) // Не начинаем перетаскивание, если идет создание соединения
            {
                isDragging = true;
                mouseOffset = e.GetPosition(this);
                this.CaptureMouse();
            }
            e.Handled = true;
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragging)
            {
                isDragging = false;
                this.ReleaseMouseCapture();
            }
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentPosition = e.GetPosition(this.Parent as Canvas);

                Canvas.SetLeft(this, currentPosition.X - mouseOffset.X);
                Canvas.SetTop(this, currentPosition.Y - mouseOffset.Y);
            }
        }

        private void ConnectorPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isConnecting = true;
            ConnectionStarted?.Invoke(this, new ConnectionEventArgs
            {
                StartControl = this,
                ConnectionPoint = ConnectorPoint
            });
            e.Handled = true;
        }

        private void ConnectorPoint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isConnecting)
            {
                isConnecting = false;
                ConnectionEnded?.Invoke(this, new ConnectionEventArgs
                {
                    StartControl = this,
                    ConnectionPoint = ConnectorPoint
                });
            }
            e.Handled = true;
        }

        public Point GetConnectorPosition()
        {
            return ConnectorPoint.TranslatePoint(
                new Point(ConnectorPoint.ActualWidth / 2, ConnectorPoint.ActualHeight / 2),
                this.Parent as Canvas);
        }
    }



    public class ConnectionEventArgs : EventArgs
    {
        public GraphNode StartControl { get; set; }
        public Ellipse ConnectionPoint { get; set; }
    }
}
