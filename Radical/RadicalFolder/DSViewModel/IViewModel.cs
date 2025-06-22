using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace DSOptimization
{
    public interface IViewModel:INotifyPropertyChanged
    {
        bool ChangesEnabled { get; set; }
        bool CheckPropertyChanged<T>(string propertyName, ref T oldValue, ref T newValue);
        void FirePropertyChanged(string propertyName);
    }
}
