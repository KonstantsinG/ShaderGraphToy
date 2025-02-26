using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace ShaderGraph.ComponentModel.Info.Wrappers
{
    public class TreeViewerItem : INotifyPropertyChanged
    {
        public ObservableCollection<TreeViewerItem> Children { get; set; } = [];
        public ICommand? ClickCommand { get; }

        private ITreeViewerItem? _model = null;
        public ITreeViewerItem? Model
        {
            get => _model;
            set
            {
                _model = value;
                OnPropertyChanged(nameof(Model));
            }
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                OnPropertyChanged(nameof(IsExpanded));
            }
        }


        public bool HasChildren => Children?.Count > 0;


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
