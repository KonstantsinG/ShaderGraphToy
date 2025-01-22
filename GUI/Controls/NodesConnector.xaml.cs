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
    /// Логика взаимодействия для NodesConnector.xaml
    /// </summary>
    public partial class NodesConnector : UserControl
    {
        public NodesConnector()
        {
            InitializeComponent();
        }


        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            outlineEllipse.Fill = (SolidColorBrush)FindResource("HighlightBlue");
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            outlineEllipse.Fill = (SolidColorBrush)FindResource("PastelGreen");
        }
    }
}
