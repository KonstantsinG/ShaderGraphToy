using System.ComponentModel;
using System.Runtime.CompilerServices;
using StbImageSharp;

namespace Nodes2Shader.GraphNodesImplementation.Components
{
    public class TextureComponent : INodeComponent, INotifyPropertyChanged
    {
        private int _width = 100;
        public int Width
        {
            get => _width;
            set
            {
                _width = value;
                OnPropertyChanged(nameof(Width));
            }
        }

        private int _height = 100;
        public int Height
        {
            get => _height;
            set
            {
                _height = value;
                OnPropertyChanged(nameof(Height));
            }
        }

        private byte[] _imageData = [];
        public byte[] ImageData
        {
            get => _imageData;
            set
            {
                _imageData = value;
                OnPropertyChanged(nameof(ImageData));
            }
        }

        private int _wrapS = 0;
        public int WrapS
        {
            get => _wrapS;
            set
            {
                _wrapS = value;
                OnPropertyChanged(nameof(WrapS));
            }
        }

        private int _wrapT = 0;
        public int WrapT
        {
            get => _wrapT;
            set
            {
                _wrapT = value;
                OnPropertyChanged(nameof(WrapT));
            }
        }

        private int _filterMag = 0;
        public int FilterMag
        {
            get => _filterMag;
            set
            {
                _filterMag = value;
                OnPropertyChanged(nameof(FilterMag));
            }
        }

        private int _filterMin = 0;
        public int FilterMin
        {
            get => _filterMin;
            set
            {
                _filterMin = value;
                OnPropertyChanged(nameof(FilterMin));
            }
        }


        public void LoadTexture(string path)
        {
            StbImage.stbi_set_flip_vertically_on_load(1);
            ImageResult image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);
            
            Width = image.Width;
            Height = image.Height;
            ImageData = image.Data;
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
