// Decompiled with JetBrains decompiler
// Type: StormCloud.StormCloudInfo
// Assembly: StormCloud, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4FE6912A-CE16-4961-A22E-FF9E4F1676BC
// Assembly location: D:\gh-stormcloud-master\gh-stormcloud-master\StormCloudAnalysis\StormCloudAnalysis\obj\Debug64\StormCloud.dll

using Grasshopper.Kernel;
using System;
using System.Drawing;

#nullable disable
namespace StormCloud
{
  public class StormCloudInfo : GH_AssemblyInfo
  {
    public override string Name => "StormCloud";

    public override Bitmap Icon => (Bitmap) null;

    public override string Description => "Interactive Evolutionary Optimization";

    public override Guid Id => new Guid("a5e2d6da-de21-47ea-8fe9-3e48aaefd1f6");

    public override string AuthorName => "Renaud Alexis P.E. Danhaive, Caitlin T. Mueller";

    public override string AuthorContact => "danhaive@mit.edu";
  }
}
