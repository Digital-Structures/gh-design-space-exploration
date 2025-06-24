// Decompiled with JetBrains decompiler
// Type: Resources.Resources
// Assembly: StormCloudAnalysis, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E0D28C9E-619E-488E-B920-E321934C44B1
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\StormCloudAnalysis\obj\Debug64\StormCloudAnalysis.dll

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
  [CompilerGenerated]
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
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
        if (object.ReferenceEquals((object) Resources.resourceMan, (object) null))
          Resources.resourceMan = new ResourceManager("StormCloudAnalysis.Properties.Resources", typeof (Resources).Assembly);
        return Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => Resources.resourceCulture;
      set => Resources.resourceCulture = value;
    }

    public static Bitmap load
    {
      get => (Bitmap) Resources.ResourceManager.GetObject(nameof (load), Resources.resourceCulture);
    }
  }
}
