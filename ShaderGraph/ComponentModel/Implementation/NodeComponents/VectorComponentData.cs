using System.ComponentModel;

namespace ShaderGraph.ComponentModel.Implementation.NodeComponents
{
    public class VectorComponentData : IGraphNodeComponent, INotifyPropertyChanged
    {
        public required string Type { get; set; }

        private string _title = "";
        public required string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        private List<string> _contents = [];
        public required List<string> Contents
        {
            get => _contents;
            set
            {
                _contents = value;
                OnPropertyChanged(nameof(Contents));
            }
        }

        private bool _isControlable = false;
        public required bool IsControlable
        {
            get => _isControlable;
            set
            {
                _isControlable= value;
                OnPropertyChanged(nameof(IsControlable));
            }
        }

        private bool _isReadonly = false;
        public required bool IsReadonly
        {
            get => _isReadonly;
            set
            {
                _isReadonly = value;
                OnPropertyChanged(nameof(IsReadonly));
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
