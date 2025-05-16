using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;


namespace ShaderGraphToy.Windows
{
    /// <summary>
    /// Логика взаимодействия для ExportWindow.xaml
    /// </summary>
    public partial class CodeExportWindow : Window
    {
        public CodeExportWindow()
        {
            InitializeComponent();
            CodeExportWindowVM vm = new() { setCode = UpdateCode };
            exportBtn.Click += vm.OnExportClick;
            vm.UpdateCode();
            DataContext = vm;

            cancelBtn.Click += OnCancelClick;
        }

        public void UpdateCode(Paragraph code)
        {
            doc.Blocks.Add(code);
        }

        public void OnCancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}