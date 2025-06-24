using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StormCloud.Evolutionary;

namespace StormCloud.ViewModel
{
    public class EvoParamsVM : BaseVM
    {
        public EvoParamsVM(EvoParams model)
        {
            this.Model = model;
        }

        //private int _PopulationSize = 0;
        public int PopulationSize
        {
            get
            {
                return Model.GenSize;
            }
            set
            {
                if (value != PopulationSize)
                {
                    Model.GenSize = value;
                    FirePropertyChanged("PopulationSize");
                }
                //CheckPropertyChanged<int>("PopulationSize", ref _PopulationSize, ref value); 
            }
        }

        //private double _MutationRate = 0;
        public double MutationRate
        {
            get
            {
                return Model.MutRate;
            }
            set
            {
                if (value != MutationRate)
                {
                    Model.MutRate = value;
                    FirePropertyChanged("MutationRate");
                }
                //double roundedRate = Math.Round(value, 2);
                //CheckPropertyChanged<double>("MutationRate", ref _MutationRate, ref roundedRate);
            }
        }

        public EvoParams Model;

        //    #region INotifyPropertyChanged

        //    public event PropertyChangedEventHandler PropertyChanged;

        //    protected bool CheckPropertyChanged<T>(string propertyName, ref T oldValue, ref T newValue)
        //    {
        //        if (oldValue == null && newValue == null)
        //        {
        //            return false;
        //        }

        //        if ((oldValue == null && newValue != null) || !oldValue.Equals((T)newValue))
        //        {
        //            oldValue = newValue;
        //            FirePropertyChanged(propertyName);
        //            return true;
        //        }

        //        return false;
        //    }

        //    protected void FirePropertyChanged(string propertyName)
        //    {
        //        if (this.PropertyChanged != null)
        //        {
        //            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //        }
        //    }

        //    #endregion
    }
}