using System.Windows;
using System.Windows.Input;

namespace ShaderGraphToy.Windows
{
    /// <summary>
    /// Логика взаимодействия для GraphNodesBrowserWindow.xaml
    /// </summary>
    public partial class GraphNodesBrowserWindow : Window
    {
        public GraphNodesBrowserWindow()
        {
            InitializeComponent();
            searchBox.Focus();

            var vm = new GraphNodesBrowserWindowVM();
            treeView.SelectedItemChanged += vm.TreeView_SelectedItemChanged;
            searchBox.TextChanged += vm.SearchBox_TextChanged;
            searchBox.PreviewKeyDown += vm.SearchBoxKeyPressed;
            DataContext = vm;
        }

        private void TreeViewItem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ((GraphNodesBrowserWindowVM)DataContext!).TreeViewItem_PreviewKeyDown(sender, e);
        }
    }
}
