using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Stepper
{
    //BASE VM
    //Base class for view models
    //Manages communication between code "Model" and UI "View" via prpoperty changes
    public class BaseVM:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        //CHANGES ENABLED
        //Determines whether optimization is running (disabling changes)
        protected bool OptRunning;
        public bool ChangesEnabled
        {
            get { return !OptRunning; }
            set { OptRunning = !value; FirePropertyChanged("ChangesEnabled"); }
        }

        //CHECK PROPERTY CHANGED
        //Called by setter methods for VM properties
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

        //FIRE PROPERTY CHANGED
        //Event to notify UI of property updates
        public void FirePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
