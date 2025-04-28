using Nodes2Shader.Compilation.MathGraph;
using Nodes2Shader.GraphNodesImplementation.Components;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ShaderGraphToy.Representation.GraphNodes.GraphNodeComponents
{
    /// <summary>
    /// Логика взаимодействия для MatrixComponent.xaml
    /// </summary>
    public partial class MatrixComponentView : UserControl, INotifyPropertyChanged, INodeComponentView
    {
        public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(
            nameof(Model), typeof(MatrixComponent), typeof(MatrixComponentView), new PropertyMetadata(null));

        public MatrixComponent Model
        {
            get { return (MatrixComponent)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }

        private bool _isThirdRowVisible = true;
        public bool IsThirdRowVisible
        {
            get => _isThirdRowVisible;
            set
            {
                _isThirdRowVisible = value;
                OnPropertyChanged(nameof(IsThirdRowVisible));
            }
        }

        private bool _isForthRowVisible = false;
        public bool IsForthRowVisible
        {
            get => _isForthRowVisible;
            set
            {
                _isForthRowVisible = value;
                OnPropertyChanged(nameof(IsForthRowVisible));
            }
        }

        private bool _isThirdColumnVisible = true;
        public bool IsThirdColumnVisible
        {
            get => _isThirdColumnVisible;
            set
            {
                _isThirdColumnVisible = value;
                OnPropertyChanged(nameof(IsThirdColumnVisible));
            }
        }

        private bool _isForthColumnVisible = false;
        public bool IsForthColumnVisible
        {
            get => _isForthColumnVisible;
            set
            {
                _isForthColumnVisible = value;
                OnPropertyChanged(nameof(IsForthColumnVisible));
            }
        }


        public MatrixComponentView()
        {
            InitializeComponent();
        }


        public NodeEntry GetData()
        {
            (string type, string value) = cBox.SelectedIndex switch
            {
                0 => ("Mat2x2", $"({tb00.Text}, {tb10.Text}, {tb01.Text}, {tb11.Text})"),
                1 => ("Mat3x2", $"({tb00.Text}, {tb10.Text}, {tb01.Text}, {tb11.Text}, {tb02.Text}, {tb12})"),
                2 => ("Mat4x2", $"({tb00.Text}, {tb10.Text}, {tb01.Text}, {tb11.Text}, {tb02.Text}, {tb12.Text}, {tb03.Text}, {tb13.Text})"),
                3 => ("Mat2x3", $"({tb00.Text}, {tb10.Text}, {tb20.Text}, {tb01.Text}, {tb11.Text}, {tb21.Text})"),
                4 => ("Mat3x3", $"({tb00.Text}, {tb10.Text}, {tb20.Text}, {tb01.Text}, {tb11.Text}, {tb21.Text}, {tb02.Text}, {tb12.Text}, {tb22.Text})"),
                5 => ("Mat4x3", $"({tb00.Text}, {tb10.Text}, {tb20.Text}, {tb30.Text}, {tb01.Text}, {tb11.Text}, {tb21.Text}, {tb31.Text}, {tb02.Text}, {tb12.Text}, {tb22.Text}, {tb32.Text})"),
                6 => ("Mat2x4", $"({tb00.Text}, {tb10.Text}, {tb01.Text}, {tb11.Text}, {tb02.Text}, {tb12.Text}, {tb03.Text}, {tb13.Text})"),
                7 => ("Mat3x4", $"({tb00.Text}, {tb10.Text}, {tb20.Text}, {tb01.Text}, {tb11.Text}, {tb21.Text}, {tb02.Text}, {tb12.Text}, {tb22.Text}, {tb03.Text}, {tb13.Text}, {tb23.Text})"),
                8 => ("Mat4x4", $"({tb00.Text}, {tb10.Text}, {tb20.Text}, {tb30.Text}, {tb01.Text}, {tb11.Text}, {tb21.Text}, {tb31.Text}, {tb02.Text}, {tb12.Text}, {tb22.Text}, {tb32.Text}, " +
                     $"{tb03.Text}, {tb13.Text}, {tb23.Text}, {tb33.Text})"),
                _ => (string.Empty, string.Empty)
            };

            return new(type, value, NodeEntry.EntryType.Value);
        }


        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbox = (ComboBox)sender;

            (IsThirdRowVisible, IsForthRowVisible, IsThirdColumnVisible, IsForthColumnVisible) = cbox.SelectedIndex switch
            {
                0 => (false, false, false, false),
                1 => (false, false, true, false),
                2 => (false, false, true, true),
                3 => (true, false, false, false),
                4 => (true, false, true, false),
                5 => (true, false, true, true),
                6 => (true, true, false, false),
                7 => (true, true, true, false),
                8 => (true, true, true, true),
                _ => (IsThirdRowVisible, IsForthRowVisible, IsThirdColumnVisible, IsForthColumnVisible)
            };
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
