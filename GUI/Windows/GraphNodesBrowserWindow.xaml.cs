using System.Windows;
using System.Windows.Input;

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
            searchBox.Focus();

            var vm = new GraphNodesBrowserWindowVM();
            treeView.SelectedItemChanged += vm.TreeView_SelectedItemChanged;
            searchBox.TextChanged += vm.SearchBox_TextChanged;
            crossRect.MouseDown += vm.CrossRect_MouseDown;

            searchBox.PreviewKeyDown += vm.SearchBoxKeyPressed;
            addButton.Click += vm.AddButton_Click;
            cancelButton.Click += vm.CancelButton_Click;
            DataContext = vm;
        }

        private void TreeViewItem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ((GraphNodesBrowserWindowVM)DataContext!).TreeViewItem_PreviewKeyDown(sender, e);
        }
    }
}
