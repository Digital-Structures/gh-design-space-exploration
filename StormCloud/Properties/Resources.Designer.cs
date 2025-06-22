// Decompiled with JetBrains decompiler
// Type: Resources.Resources
// Assembly: StormCloud, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4FE6912A-CE16-4961-A22E-FF9E4F1676BC
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\StormCloudAnalysis\obj\Debug64\StormCloud.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

#nullable disable
namespace Resources
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (Resources.Resources.resourceMan == null)
          Resources.Resources.resourceMan = new ResourceManager("StormCloud.Properties.Resources", typeof (Resources.Resources).Assembly);
        return Resources.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => Resources.Resources.resourceCulture;
      set => Resources.Resources.resourceCulture = value;
    }

    public static Bitmap gen_icon
    {
      get
      {
        return (Bitmap) Resources.Resources.ResourceManager.GetObject(nameof (gen_icon), Resources.Resources.resourceCulture);
      }
    }

    public static Bitmap gen_icon_small
    {
      get
      {
        return (Bitmap) Resources.Resources.ResourceManager.GetObject(nameof (gen_icon_small), Resources.Resources.resourceCulture);
      }
    }
  }
}
