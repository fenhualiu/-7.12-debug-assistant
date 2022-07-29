using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace _7._12_debug_assistant
{
    public class INotifyPropertyChang : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string p)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(p));
            }
        }
        public void RaiseAndSetIfChanged<T>(ref T a, T v, [CallerMemberName] string propertyName = null)
        {
            a = v;
            if (propertyName != null)
            {
                RaisePropertyChanged(propertyName);
            }
        }
    }
}
