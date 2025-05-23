﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ShaderGraphToy.Utilities.DataBindings
{
    public class VmBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
