using Nodes2Shader.Compilation.MathGraph;
using Nodes2Shader.DataTypes;
using Nodes2Shader.GraphNodesImplementation.Components;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ShaderGraphToy.Representation.GraphNodes.GraphNodeComponents
{
    /// <summary>
    /// Логика взаимодействия для VectorComponent.xaml
    /// </summary>
    public partial class VectorComponentView : UserControl, INotifyPropertyChanged, INodeComponentView
    {
        public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(
            nameof(Model), typeof(VectorComponent), typeof(VectorComponentView), new PropertyMetadata(null));

        public VectorComponent Model
        {
            get { return (VectorComponent)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }

        private bool _isThirdVisible = true;
        public bool IsThirdVisible
        {
            get => _isThirdVisible;
            set
            {
                _isThirdVisible = value;
                OnPropertyChanged(nameof(IsThirdVisible));
            }
        }

        private bool _isForthVisible = false;
        public bool IsForthVisible
        {
            get => _isForthVisible;
            set
            {
                _isForthVisible = value;
                OnPropertyChanged(nameof(IsForthVisible));
            }
        }


        public VectorComponentView()
        {
            InitializeComponent();
        }


        public NodeEntry GetData()
        {
            string type, value;
            
            switch (cBox.SelectedIndex)
            {
                case 0:
                    ValidateTextBoxes(tb0, tb1);

                    type = "Vec2";
                    value = $"vec2({tb0.Text}, {tb1.Text})";
                    break;

                case 1:
                    ValidateTextBoxes(tb0, tb1, tb2);

                    type = "Vec3";
                    value = $"vec3({tb0.Text}, {tb1.Text}, {tb2.Text})";
                    break;

                case 2:
                    ValidateTextBoxes(tb0, tb1, tb2, tb3);

                    type = "Vec4";
                    value = $"vec4({tb0.Text}, {tb1.Text}, {tb2.Text}, {tb3.Text})";
                    break;

                default:
                    throw new NotImplementedException("A list component cannot have this value as a selected value.");
            }

            return new NodeEntry(type, value, NodeEntry.EntryType.Value);
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

            (IsThirdVisible, IsForthVisible) = cbox.SelectedIndex switch
            {
                0 => (false, false),
                1 => (true, false),
                2 => (true, true),
                _ => (IsThirdVisible, IsForthVisible)
            };
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
