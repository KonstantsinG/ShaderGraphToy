using ShaderGraph.ComponentModel.Implementation.NodeComponents;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Controls.GraphNodeComponents
{
    /// <summary>
    /// Логика взаимодействия для MatrixComponent.xaml
    /// </summary>
    public partial class MatrixComponent : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(
            nameof(Model), typeof(MatrixComponentData), typeof(MatrixComponent), new PropertyMetadata(null));

        public MatrixComponentData Model
        {
            get { return (MatrixComponentData)GetValue(ModelProperty); }
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


        public MatrixComponent()
        {
            InitializeComponent();
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
