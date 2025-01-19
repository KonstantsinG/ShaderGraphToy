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
        public Brush HeaderColor
        {
            get { return (Brush)GetValue(HeaderColorProperty); }
            set { SetValue(HeaderColorProperty, value); }
        }

        public static readonly DependencyProperty HeaderColorProperty =
            DependencyProperty.Register("HeaderColor", typeof(Brush), typeof(GraphNodeBase), new PropertyMetadata(Brushes.Bisque));

        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register("HeaderText", typeof(string), typeof(GraphNodeBase), new PropertyMetadata("Node Type"));



        public GraphNodeBase()
        {
            InitializeComponent();
        }
    }
}
