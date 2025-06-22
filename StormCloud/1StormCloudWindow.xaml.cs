// Decompiled with JetBrains decompiler
// Type: StormCloud.StringFormatConverter
// Assembly: StormCloud, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4FE6912A-CE16-4961-A22E-FF9E4F1676BC
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\StormCloudAnalysis\obj\Debug64\StormCloud.dll

using System;
using System.Globalization;
using System.Windows.Data;

#nullable disable
namespace StormCloud
{
  public class StringFormatConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string format = parameter as string;
      if (string.IsNullOrEmpty(format))
        return (object) value.ToString();
      return (object) string.Format((IFormatProvider) culture, format, new object[1]
      {
        value
      });
    }

    public object ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      return (object) double.Parse((string) value);
    }
  }
}
