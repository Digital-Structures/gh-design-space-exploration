using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Stepper
{
    // Token: 0x0200000D RID: 13
    public class BaseVM : IViewModel, INotifyPropertyChanged
    {
        // Token: 0x14000001 RID: 1
        // (add) Token: 0x06000067 RID: 103 RVA: 0x00002C2C File Offset: 0x00000E2C
        // (remove) Token: 0x06000068 RID: 104 RVA: 0x00002C64 File Offset: 0x00000E64
        public event PropertyChangedEventHandler PropertyChanged;

        // Token: 0x17000029 RID: 41
        // (get) Token: 0x06000069 RID: 105 RVA: 0x00002C9C File Offset: 0x00000E9C
        // (set) Token: 0x0600006A RID: 106 RVA: 0x00002CB7 File Offset: 0x00000EB7
        public bool ChangesEnabled
        {
            get
            {
                return !this.OptRunning;
            }
            set
            {
                this.OptRunning = !value;
                this.FirePropertyChanged("ChangesEnabled");
            }
        }

        // Token: 0x0600006B RID: 107 RVA: 0x00002CD0 File Offset: 0x00000ED0
        public bool CheckPropertyChanged<T>(string propertyName, ref T oldValue, ref T newValue)
        {
            bool flag = oldValue == null && newValue == null;
            bool result;
            if (flag)
            {
                result = false;
            }
            else
            {
                bool flag2 = (oldValue == null && newValue != null) || !oldValue.Equals(newValue);
                if (flag2)
                {
                    oldValue = newValue;
                    this.FirePropertyChanged(propertyName);
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            return result;
        }

        // Token: 0x0600006C RID: 108 RVA: 0x00002D60 File Offset: 0x00000F60
        public void FirePropertyChanged(string propertyName)
        {
            bool flag = this.PropertyChanged != null;
            if (flag)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        // Token: 0x0400001F RID: 31
        protected bool OptRunning;
    }
}
