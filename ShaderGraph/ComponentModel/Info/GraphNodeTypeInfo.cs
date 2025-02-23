using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderGraph.ComponentModel.Info
{
    public class GraphNodeTypeInfo : INotifyPropertyChanged
    {
        private string _name = "";
        public required string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private string _description = "";
        public required string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public required List<string> Synonyms { get; set; }
        public required int TypeId { get; set; }

        private string _color = "#FF51B143";
        public required string Color
        {
            get => _color;
            set
            {
                if (value != null) _color = value;
                else _color = "#FF51B143";

                OnPropertyChanged(nameof(Color));
            }
        }

        private bool _usingOperations = true;
        public required bool UsingOperations
        {
            get => _usingOperations;
            set
            {
                _usingOperations = value;
                OnPropertyChanged(nameof(UsingOperations));
            }
        }

        private bool _usingSubOperations = true;
        public required bool UsingSubOperations
        {
            get => _usingSubOperations;
            set
            {
                _usingSubOperations = value;
                OnPropertyChanged(nameof(UsingSubOperations));
            }
        }

        public required List<GraphNodeOperationInfo> OperationsTypes { get; set; }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
