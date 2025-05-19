using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Nodes2Shader.GraphNodesImplementation.Components
{
    public class MatrixComponent : INodeComponent, INotifyPropertyChanged
    {
        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        private List<List<string>> _contents = [];
        public List<List<string>> Contents
        {
            get => _contents;
            set
            {
                _contents = value;
                OnPropertyChanged(nameof(Contents));
            }
        }

        private bool _isReadonly = false;
        public bool IsReadonly
        {
            get => _isReadonly;
            set
            {
                _isReadonly = value;
                OnPropertyChanged(nameof(IsReadonly));
            }
        }

        private bool _isControlable = true;
        public bool IsControlable
        {
            get => _isControlable;
            set
            {
                _isControlable = value;
                OnPropertyChanged(nameof(IsControlable));
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
