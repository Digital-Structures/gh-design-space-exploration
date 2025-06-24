// Decompiled with JetBrains decompiler
// Type: StructureEngine.Logging.DefaultRequestController
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

#nullable disable
namespace StructureEngine.Logging
{
  public class DefaultRequestController : IRequestController
  {
    public virtual int MaxAttempts => 1;

    public virtual bool OnSendFailure(Exception ex)
    {
      bool flag = false;
      switch (ex)
      {
        case WebException _:
          flag = true;
          break;
        case IOException _:
          flag = true;
          break;
        case SocketException _:
          flag = true;
          break;
      }
      if (!flag)
        return false;
      this.RecordRetryError(ex);
      return true;
    }

    public virtual void RecordRetryError(Exception ex)
    {
    }

    public virtual void OnBeforeSendRequest(HttpWebRequest request)
    {
    }

    public virtual void OnEndSendRequest(HttpWebRequest request, Exception sendException)
    {
    }
  }
}
