using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Components
{
    internal class RenderingViewportVM : INotifyPropertyChanged
    {
        public void OnPause_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var t = 23847;
        }



        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
