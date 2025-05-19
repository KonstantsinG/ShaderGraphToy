using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Nodes2Shader.GraphNodesImplementation.Components
{
    public class InscriptionComponent : INodeComponent, INotifyPropertyChanged
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

        private string _defaultInput = "Ignore";
        public string DefaultInput
        {
            get => _defaultInput;
            set
            {
                _defaultInput = value;
                OnPropertyChanged(nameof(DefaultInput));
            }
        }

        private List<string> _formatting = [];
        public List<string> Formatting
        {
            get => _formatting;
            set
            {
                _formatting = value;
                OnPropertyChanged(nameof(Formatting));
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

        private bool _hasOutput = true;
        public bool HasOutput
        {
            get => _hasOutput;
            set
            {
                _hasOutput = value;
                OnPropertyChanged(nameof(HasOutput));
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

        private string _outputType = string.Empty;
        public string OutputType
        {
            get => _outputType;
            set
            {
                _outputType = value;
                OnPropertyChanged(nameof(OutputType));
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
