// Decompiled with JetBrains decompiler
// Type: StormCloud.BoolObsCollConverter
// Assembly: StormCloud, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4FE6912A-CE16-4961-A22E-FF9E4F1676BC
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\StormCloudAnalysis\obj\Debug64\StormCloud.dll

using System;
using System.Globalization;
using System.Windows.Data;

#nullable disable
namespace StormCloud
{
  public class BoolObsCollConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (object) ((int) value > 0);
    }

    public object ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
