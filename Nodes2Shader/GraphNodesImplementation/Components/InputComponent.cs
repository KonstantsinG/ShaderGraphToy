using Nodes2Shader.DataTypes;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Nodes2Shader.GraphNodesImplementation.Components
{
    public class InputComponent : INodeComponent, INotifyPropertyChanged
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

        private string _content = string.Empty;
        public string Content
        {
            get => _content;
            set
            {
                _content = value;
                OnPropertyChanged(nameof(Content));
            }
        }

        private bool _hasInput = true;
        public bool HasInput
        {
            get => _hasInput;
            set
            {
                _hasInput = value;
                OnPropertyChanged(nameof(HasInput));
            }
        }

        private string _inputType = string.Empty;
        public string InputType
        {
            get => _inputType;
            set
            {
                _inputType = value;
                OnPropertyChanged(nameof(InputType));
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
