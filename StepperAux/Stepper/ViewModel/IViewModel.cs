using System;
using System.ComponentModel;

namespace Stepper
{
    // Token: 0x02000010 RID: 16
    public interface IViewModel : INotifyPropertyChanged
    {
        // Token: 0x17000031 RID: 49
        // (get) Token: 0x06000080 RID: 128
        // (set) Token: 0x06000081 RID: 129
        bool ChangesEnabled { get; set; }

        // Token: 0x06000082 RID: 130
        bool CheckPropertyChanged<T>(string propertyName, ref T oldValue, ref T newValue);

        // Token: 0x06000083 RID: 131
        void FirePropertyChanged(string propertyName);
    }
}
