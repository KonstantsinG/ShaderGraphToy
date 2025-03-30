using ShaderGraph.GraphNodesImplementation.Components;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Representation.GraphNodes.GraphNodeComponents
{
    /// <summary>
    /// Логика взаимодействия для VectorComponent.xaml
    /// </summary>
    public partial class VectorComponentView : UserControl, INotifyPropertyChanged
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
