using ShaderGraph.ComponentModel.Implementation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GUI.Controls.GraphNodeComponents
{
    /// <summary>
    /// Логика взаимодействия для VectorComponent.xaml
    /// </summary>
    public partial class VectorComponent : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(
            nameof(Model), typeof(VectorComponentData), typeof(VectorComponent), new PropertyMetadata(null));

        public VectorComponentData Model
        {
            get { return (VectorComponentData)GetValue(ModelProperty); }
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


        public VectorComponent()
        {
            InitializeComponent();

            VectorComponentData data = new()
            {
                Title = "Vector TEST",
                Contents = [ "0.15", "0.16" ],
                IsReadonly = true,
                IsControlable = true
            };
            Model = data;
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
