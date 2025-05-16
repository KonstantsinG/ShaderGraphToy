using Nodes2Shader.Compilation.MathGraph;
using Nodes2Shader.DataTypes;
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
            string type, value;

            switch (cBox.SelectedIndex)
            {
                case 0:
                    ValidateTextBoxes(tb00, tb10, tb01, tb11);

                    type = "Mat2";
                    value = $"mat2({tb00.Text}, {tb10.Text}, {tb01.Text}, {tb11.Text})";
                    break;

                case 1:
                    ValidateTextBoxes(tb00, tb10, tb01, tb11, tb02, tb12);

                    type = "Mat3x2";
                    value = $"mat3x2({tb00.Text}, {tb10.Text}, {tb01.Text}, {tb11.Text}, {tb02.Text}, {tb12})";
                    break;

                case 2:
                    ValidateTextBoxes(tb00, tb10, tb01, tb11, tb02, tb12, tb03, tb13);

                    type = "Mat4x2";
                    value = $"mat4x2({tb00.Text}, {tb10.Text}, {tb01.Text}, {tb11.Text}, {tb02.Text}, {tb12.Text}, {tb03.Text}, {tb13.Text})";
                    break;

                case 3:
                    ValidateTextBoxes(tb00, tb10, tb20, tb01, tb11, tb21);

                    type = "Mat2x3";
                    value = $"mat2x3({tb00.Text}, {tb10.Text}, {tb20.Text}, {tb01.Text}, {tb11.Text}, {tb21.Text})";
                    break;

                case 4:
                    ValidateTextBoxes(tb00, tb10, tb20, tb01, tb11, tb21, tb02, tb12, tb22);

                    type = "Mat3";
                    value = $"mat3({tb00.Text}, {tb10.Text}, {tb20.Text}, {tb01.Text}, {tb11.Text}, {tb21.Text}, {tb02.Text}, {tb12.Text}, {tb22.Text})";
                    break;

                case 5:
                    ValidateTextBoxes(tb00, tb10, tb20, tb30, tb01, tb11, tb21, tb31, tb02, tb12, tb22, tb32);

                    type = "Mat4x3";
                    value = $"mat4x3({tb00.Text}, {tb10.Text}, {tb20.Text}, {tb30.Text}, {tb01.Text}, {tb11.Text}, {tb21.Text}, {tb31.Text}, {tb02.Text}, {tb12.Text}, {tb22.Text}, {tb32.Text})";
                    break;

                case 6:
                    ValidateTextBoxes(tb00, tb10, tb01, tb11, tb02, tb12, tb03, tb13);

                    type = "Mat2x4";
                    value = $"mat2x4({tb00.Text}, {tb10.Text}, {tb01.Text}, {tb11.Text}, {tb02.Text}, {tb12.Text}, {tb03.Text}, {tb13.Text})";
                    break;

                case 7:
                    ValidateTextBoxes(tb00, tb10, tb20, tb01, tb11, tb21, tb02, tb12, tb22, tb03, tb13, tb23);

                    type = "Mat3x4";
                    value = $"mat3x4({tb00.Text}, {tb10.Text}, {tb20.Text}, {tb01.Text}, {tb11.Text}, {tb21.Text}, {tb02.Text}, {tb12.Text}, {tb22.Text}, {tb03.Text}, {tb13.Text}, {tb23.Text})";
                    break;

                case 8:
                    ValidateTextBoxes(tb00, tb10, tb20, tb30, tb01, tb11, tb21, tb31, tb02, tb12, tb22, tb32, tb03, tb13, tb23, tb33);

                    type = "Mat4";
                    value = $"mat4({tb00.Text}, {tb10.Text}, {tb20.Text}, {tb30.Text}, {tb01.Text}, {tb11.Text}, {tb21.Text}, {tb31.Text}, {tb02.Text}, {tb12.Text}, {tb22.Text}, {tb32.Text}, " +
                            $"{tb03.Text}, {tb13.Text}, {tb23.Text}, {tb33.Text})";
                    break;

                default:
                    throw new NotImplementedException("A list component cannot have this value as a selected value.");
            }

            return new(type, value, NodeEntry.EntryType.Value);
        }

        private static void ValidateTextBoxes(params TextBox[] tbs)
        {
            for (int i = 0; i < tbs.Length; i++)
            {
                if (!DataTypesConverter.IsNumberValid(tbs[i].Text))
                    throw new FormatException($"TextBox {i + 1} contains invalid number!");

                if (!tbs[i].Text.Contains('.'))
                    tbs[i].Text = tbs[i].Text + ".0";
            }
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
