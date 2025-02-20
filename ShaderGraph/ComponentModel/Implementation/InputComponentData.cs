using ShaderGraph.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderGraph.ComponentModel.Implementation
{
    public class InputComponentData : IGraphNodeComponent, INotifyPropertyChanged
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

        private string _content = "";
        public required string Content
        {
            get => _content;
            set
            {
                _content = value;
                OnPropertyChanged(nameof(Content));
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

        private VariantConverter.DataType? _inputType = null;
        public required VariantConverter.DataType? InputType
        {
            get => _inputType;
            set
            {
                _inputType = value;
                OnPropertyChanged(nameof(InputType));
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
