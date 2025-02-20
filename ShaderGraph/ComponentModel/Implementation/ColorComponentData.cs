using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderGraph.ComponentModel.Implementation
{
    public class ColorComponentData : IGraphNodeComponent, INotifyPropertyChanged
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

        private string _content = "#FF535761";
        public required string Content
        {
            get => _content;
            set
            {
                if (value != null) _content = value;
                else _content = "#FF535761";

                OnPropertyChanged(nameof(Content));
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
