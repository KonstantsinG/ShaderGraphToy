using ShaderGraph.Converters;
using System.ComponentModel;

namespace ShaderGraph.ComponentModel.Implementation.NodeComponents
{
    public class InscriptionComponentData : IGraphNodeComponent, INotifyPropertyChanged
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

        private bool _hasInput = false;
        public required bool HasInput
        {
            get => _hasInput;
            set
            {
                _hasInput = value;
                OnPropertyChanged(nameof(HasInput));
            }
        }

        private bool _hasOutput = false;
        public required bool HasOutput
        {
            get => _hasOutput;
            set
            {
                _hasOutput = value;
                OnPropertyChanged(nameof(HasOutput));
            }
        }

        private VariantConverter.DataType? _inputType;
        public required VariantConverter.DataType? InputType
        {
            get => _inputType;
            set
            {
                _inputType = value;
                OnPropertyChanged(nameof(InputType));
            }
        }

        private VariantConverter.DataType? _outputType;
        public required VariantConverter.DataType? OutputType
        {
            get => _outputType;
            set
            {
                _outputType = value;
                OnPropertyChanged(nameof(OutputType));
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
