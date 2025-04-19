using Nodes2Shader.GraphNodesImplementation.Components;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ShaderGraphToy.Representation.GraphNodes.GraphNodeComponents
{
    /// <summary>
    /// Логика взаимодействия для ColorComponent.xaml
    /// </summary>
    public partial class ColorComponentView : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(
            nameof(Model), typeof(ColorComponent), typeof(ColorComponentView), new PropertyMetadata(null));


        public delegate void ComponentStateHandler();
        public event ComponentStateHandler ComponentSizeChanged = delegate { };


        public ColorComponent Model
        {
            get { return (ColorComponent)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }

        private bool _slidersVisible = false;
        public bool SlidersVisible
        {
            get => _slidersVisible;
            set
            {
                _slidersVisible = value;
                OnPropertyChanged(nameof(SlidersVisible));
            }
        }


        public ColorComponentView()
        {
            InitializeComponent();
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void Rectangle_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SlidersVisible = !SlidersVisible;

            UpdateLayout();
            ComponentSizeChanged.Invoke();
        }
    }
}
