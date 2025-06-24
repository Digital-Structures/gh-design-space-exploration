// Decompiled with JetBrains decompiler
// Type: StormCloud.ViewModel.BaseVM
// Assembly: StormCloud, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4FE6912A-CE16-4961-A22E-FF9E4F1676BC
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\StormCloudAnalysis\obj\Debug64\StormCloud.dll

using System.ComponentModel;

#nullable disable
namespace StormCloud.ViewModel
{
  public class BaseVM : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    protected bool CheckPropertyChanged<T>(string propertyName, ref T oldValue, ref T newValue)
    {
      if ((object) oldValue == null && (object) newValue == null || ((object) oldValue != null || (object) newValue == null) && oldValue.Equals((object) newValue))
        return false;
      oldValue = newValue;
      this.FirePropertyChanged(propertyName);
      return true;
    }

    protected void FirePropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
