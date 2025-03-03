using System.Windows;

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
            crossRect.MouseDown += vm.CrossRect_MouseDown;

            addButton.Click += vm.AddButton_Click;
            cancelButton.Click += vm.CancelButton_Click;
            DataContext = vm;
        }
    }
}
