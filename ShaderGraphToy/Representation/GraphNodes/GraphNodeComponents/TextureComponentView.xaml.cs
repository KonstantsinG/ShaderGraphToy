using Nodes2Shader.Compilation.MathGraph;
using Nodes2Shader.GraphNodesImplementation.Components;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Microsoft.Win32;

namespace ShaderGraphToy.Representation.GraphNodes.GraphNodeComponents
{
    /// <summary>
    /// Логика взаимодействия для TextureComponentView.xaml
    /// </summary>
    public partial class TextureComponentView : UserControl, INodeComponentView
    {
        public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(
           nameof(Model), typeof(TextureComponent), typeof(TextureComponentView), new PropertyMetadata(null));

        public TextureComponent Model
        {
            get => (TextureComponent)GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }

        public TextureComponentView()
        {
            InitializeComponent();

            conn.IsInput = true;
        }

        public NodesConnector GetConnector() => conn;

        public string GetContent() => string.Empty;
        public void SetContent(string _)
        {
            if (Model != null && Model.ImageData.Length > 0)
                SetBitmapFromBytes(Model.ImageData, Model.Width, Model.Height);
        }

        public NodeEntry? GetData()
        {
            if (!conn.IsBusy)
                throw new ArgumentException($"Texture component must have input coordinates!");
            if (Model.ImageData.Length == 0)
                throw new ArgumentException($"Texture component must have a texture!");

            NodeEntry entry = new()
            {
                Id = conn.ConnectorId,
                Value = "ToReveal",
                Type = "Vec2",
                Behavior = NodeEntry.EntryType.Input
            };

            return entry;
        }

        private void SetBitmapFromBytes(byte[] rgbaData, int width, int height)
        {
            var bitmap = BitmapSource.Create(
                width,
                height,
                96,
                96,
                PixelFormats.Pbgra32,
                null,
                rgbaData,
                width * 4
            );

            imgViewer.Source = bitmap;
        }

        private void ImgViewer_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Файлы изображений|*.jpeg;*.png;*.tga;*.bmp;*.psd;*.gif;*.pic;*.pnm",
                ValidateNames = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                Model.LoadTexture(openFileDialog.FileName);
                SetBitmapFromBytes(Model.ImageData, Model.Width, Model.Height);
            }
        }
    }
}
