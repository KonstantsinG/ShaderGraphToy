using Nodes2Shader.Compilation.MathGraph;
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
            (string type, string value) = cBox.SelectedIndex switch
            {
                0 => ("Vec2", $"({tb0.Text}, {tb1.Text})"),
                1 => ("Vec3", $"({tb0.Text}, {tb1.Text}, {tb2.Text})"),
                2 => ("Vec4", $"({tb0.Text}, {tb1.Text}, {tb2.Text}, {tb3.Text})"),
                _ => (string.Empty, string.Empty)
            };

            return new NodeEntry(type, value, NodeEntry.EntryType.Value);
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
