using Nodes2Shader.Compilation.MathGraph;
using Nodes2Shader.DataTypes;
using Nodes2Shader.GraphNodesImplementation.Components;
using ShaderGraphToy.Utilities.XamlConverters;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ShaderGraphToy.Representation.GraphNodes.GraphNodeComponents
{
    /// <summary>
    /// Логика взаимодействия для ColorComponent.xaml
    /// </summary>
    public partial class ColorComponentView : UserControl, INotifyPropertyChanged, INodeComponentView
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


        public static string ConvertHexToRgba(string colorHex)
        {
            if (colorHex == string.Empty) return "vec4(1.0, 1.0, 1.0, 1.0)";

            colorHex = colorHex.Trim().Replace("#", "").Replace(" ", "");
            if (colorHex.Length == 6) colorHex = "FF" + colorHex;

            byte a = Convert.ToByte(colorHex[0..2], 16);
            byte r = Convert.ToByte(colorHex[2..4], 16);
            byte g = Convert.ToByte(colorHex[4..6], 16);
            byte b = Convert.ToByte(colorHex[6..8], 16);

            string rstr = DataTypesConverter.FormatFloat(r / 255f);
            string gstr = DataTypesConverter.FormatFloat(g / 255f);
            string bstr = DataTypesConverter.FormatFloat(b / 255f);
            string astr = DataTypesConverter.FormatFloat(a / 255f);

            return $"vec4({rstr}, {gstr}, {bstr}, {astr})";
        }


        public string GetContent() => Model.Content;
        public void SetContent(string content) => Model.Content = content;

        public NodeEntry GetData()
        {
            return new("Vec4", ConvertHexToRgba(Model.Content), NodeEntry.EntryType.Value);
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SlidersVisible = !SlidersVisible;

            UpdateLayout();
            ComponentSizeChanged.Invoke();
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
