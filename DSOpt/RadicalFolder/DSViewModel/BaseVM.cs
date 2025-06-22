using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace DSOptimization
{
    public class BaseVM:IViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        //CHANGES ENABLED
        //Determines whether optimization is running (disabling changes)
        protected bool OptRunning;
        public bool ChangesEnabled
        {
            get { return !OptRunning; }
            set{ OptRunning = !value; FirePropertyChanged("ChangesEnabled"); }
        }

        public bool CheckPropertyChanged<T>(string propertyName, ref T oldValue, ref T newValue)
        {
            if (oldValue == null && newValue == null)
            {
                return false;
            }

            if ((oldValue == null && newValue != null) || !oldValue.Equals((T)newValue))
            {
                oldValue = newValue;
                FirePropertyChanged(propertyName);
                return true;
            }

            return false;
        }

        public void FirePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
