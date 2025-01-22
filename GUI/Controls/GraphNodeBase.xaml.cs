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
    /// Логика взаимодействия для GraphNodeBase.xaml
    /// </summary>
    public partial class GraphNodeBase : UserControl
    {
        public object NodeContent
        {
            get { return GetValue(NodeContentProperty); }
            set { SetValue(NodeContentProperty, value); }
        }

        public static readonly DependencyProperty NodeContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(GraphNodeBase), new PropertyMetadata(null));

        public Brush HeaderColor
        {
            get { return (Brush)GetValue(HeaderColorProperty); }
            set { SetValue(HeaderColorProperty, value); }
        }

        public static readonly DependencyProperty HeaderColorProperty =
            DependencyProperty.Register("HeaderColor", typeof(Brush), typeof(GraphNodeBase), new PropertyMetadata(Brushes.IndianRed));

        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register("HeaderText", typeof(string), typeof(GraphNodeBase), new PropertyMetadata("Node Type"));


        private bool _isTaken = false;
        private Point _mouseOffset;


        public GraphNodeBase()
        {
            InitializeComponent();
        }


        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            borderRect.Fill = (Brush)FindResource("HighlightBlue");
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            borderRect.Fill = (Brush)FindResource("Gray_00");
        }

        private void headerPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isTaken = true;
            _mouseOffset = e.GetPosition(this);
            headerPanel.CaptureMouse();
        }

        private void headerPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isTaken = false;
            headerPanel.ReleaseMouseCapture();
        }

        private void headerPanel_MouseMove(object sender, MouseEventArgs e)
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
