using GUI.Utilities;
using ShaderGraph.ComponentModel.Info.Wrappers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace GUI.Windows
{
    /// <summary>
    /// Логика взаимодействия для GraphNodesBrowserWindow.xaml
    /// </summary>
    public partial class GraphNodesBrowserWindow : Window
    {
        public GraphNodesBrowserWindow()
        {
            InitializeComponent();

            var vm = new GraphNodesBrowserWindowVM();
            treeView.SelectedItemChanged += vm.TreeView_SelectedItemChanged;
            searchBox.TextChanged += vm.SearchBox_TextChanged;
            DataContext = vm;
        }
    }
}
