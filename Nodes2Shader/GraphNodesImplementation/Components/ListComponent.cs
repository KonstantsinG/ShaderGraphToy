using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Nodes2Shader.GraphNodesImplementation.Components
{
    public class ListComponent : INodeComponent, INotifyPropertyChanged
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

        private List<string> _contents = [];
        public List<string> Contents
        {
            get => _contents;
            set
            {
                _contents = value;
                OnPropertyChanged(nameof(Contents));
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
