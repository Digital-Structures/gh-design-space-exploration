// Decompiled with JetBrains decompiler
// Type: StructureEngine.Logging.HttpWebRequestBuilder
// Assembly: StructureEngineCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B9005538-48AF-4120-9D91-DA4A9DAA247C
// Assembly location: C:\Users\vipul\Downloads\StructureEngineCS.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

#nullable disable
namespace StructureEngine.Logging
{
  public class HttpWebRequestBuilder
  {
    private Uri _requestUri;
    private WebHeaderCollection _webHeaderCollection;
    private Dictionary<string, string> _formData;
    private List<HttpWebRequestBuilder.FileEntry> _files;

    public HttpWebRequestBuilder(string url)
      : this(new Uri(url))
    {
    }

    public HttpWebRequestBuilder(Uri uri)
    {
      this._requestUri = uri;
      this._webHeaderCollection = new WebHeaderCollection();
      this._formData = new Dictionary<string, string>();
      this._files = new List<HttpWebRequestBuilder.FileEntry>();
    }

    public HttpWebResponse SendRequest()
    {
      return this.SendRequest((IRequestController) new DefaultRequestController());
    }

    public HttpWebResponse SendRequest(IRequestController controller)
    {
      string mimeBounday = "--" + Guid.NewGuid().ToString("N");
      byte[] bytes = Encoding.UTF8.GetBytes("--" + mimeBounday + "--\r\n");
      List<HttpWebRequestBuilder.IMimeSection> mimeSectionList = new List<HttpWebRequestBuilder.IMimeSection>();
      try
      {
        if (this._formData.Count > 0)
          mimeSectionList.Add((HttpWebRequestBuilder.IMimeSection) new HttpWebRequestBuilder.FormDataMimeSection(mimeBounday, this._formData));
        foreach (HttpWebRequestBuilder.FileEntry file in this._files)
          mimeSectionList.Add((HttpWebRequestBuilder.IMimeSection) new HttpWebRequestBuilder.FileMimeSection(mimeBounday, file));
        if (mimeSectionList.Count == 0)
          throw new InvalidOperationException("Cannot create request, no mime sections created");
        long num1 = 0;
        foreach (HttpWebRequestBuilder.IMimeSection mimeSection in mimeSectionList)
          num1 += mimeSection.Length;
        long num2 = num1 + (long) bytes.Length;
        int maxAttempts = controller.MaxAttempts;
        if (maxAttempts <= 0)
          throw new Exception("MaxAttempts must be greater than zero");
        for (int index = 0; index < maxAttempts; ++index)
        {
          try
          {
            HttpWebRequest request = WebRequest.Create(this._requestUri) as HttpWebRequest;
            request.Headers = this._webHeaderCollection;
            request.Method = "POST";
            request.ContentType = "multipart/form-data; boundary=" + mimeBounday;
            request.ContentLength = num2;
            Exception sendException = (Exception) null;
            try
            {
              controller.OnBeforeSendRequest(request);
              Stream requestStream1;
              try
              {
                IAsyncResult requestStream2 = request.BeginGetRequestStream((AsyncCallback) null, (object) null);
                requestStream1 = request.EndGetRequestStream(requestStream2);
              }
              catch (Exception ex)
              {
                throw;
              }
              try
              {
                foreach (HttpWebRequestBuilder.IMimeSection mimeSection in mimeSectionList)
                  mimeSection.WriteToStream(requestStream1);
                requestStream1.Write(bytes, 0, bytes.Length);
              }
              finally
              {
                requestStream1.Close();
              }
              request.BeginGetResponse((AsyncCallback) (r => { }), (object) null);
            }
            catch (Exception ex)
            {
              sendException = ex;
              throw;
            }
            finally
            {
              controller.OnEndSendRequest(request, sendException);
            }
          }
          catch (Exception ex)
          {
            if (index < maxAttempts - 1)
            {
              if (!controller.OnSendFailure(ex))
                throw;
            }
            else
              throw;
          }
        }
      }
      finally
      {
        foreach (HttpWebRequestBuilder.IMimeSection mimeSection in mimeSectionList)
        {
          try
          {
            mimeSection.Dispose();
          }
          catch (Exception ex)
          {
          }
        }
      }
      return (HttpWebResponse) null;
    }

    public WebHeaderCollection Headers => this._webHeaderCollection;

    public Dictionary<string, string> FormData => this._formData;

    public void AddStringFile(string fieldName, string fileName, string contents)
    {
      this._files.Add((HttpWebRequestBuilder.FileEntry) new HttpWebRequestBuilder.StringFileEntry(fieldName, fileName, contents));
    }

    public void AddFile(string fieldName, string file)
    {
      this._files.Add(new HttpWebRequestBuilder.FileEntry(fieldName, file));
    }

    public void AddFile(string fieldName, string file, string clientFileName)
    {
      this._files.Add(new HttpWebRequestBuilder.FileEntry(fieldName, file, clientFileName));
    }

    public void AddFile(string fieldName, string file, string clientFileName, string contentType)
    {
      this._files.Add(new HttpWebRequestBuilder.FileEntry(fieldName, file, clientFileName, contentType));
    }

    private class StringFileEntry : HttpWebRequestBuilder.FileEntry
    {
      public string Contents { get; private set; }

      public StringFileEntry(string fieldName, string fileName, string content)
        : base(fieldName, (string) null, fileName)
      {
        this.Contents = content;
      }
    }

    private class FileEntry
    {
      private const string DEFAULT_CONTENT_TYPE = "application/octet-stream";

      public string File { get; private set; }

      public string FieldName { get; private set; }

      public string ClientFileName { get; private set; }

      public string ContentType { get; private set; }

      public FileEntry(string fieldName, string file)
      {
        this.FieldName = fieldName;
        this.File = file;
        this.ClientFileName = Path.GetFileName(file);
        this.ContentType = "application/octet-stream";
      }

      public FileEntry(string fieldName, string file, string clientFilename)
      {
        this.FieldName = fieldName;
        this.File = file;
        this.ClientFileName = clientFilename;
        this.ContentType = "application/octet-stream";
      }

      public FileEntry(string fieldName, string file, string clientFilename, string contentType)
      {
        this.FieldName = fieldName;
        this.File = file;
        this.ClientFileName = clientFilename;
        this.ContentType = string.IsNullOrEmpty(contentType) ? "application/octet-stream" : contentType;
      }
    }

    private interface IMimeSection : IDisposable
    {
      long Length { get; }

      void WriteToStream(Stream requestStream);
    }

    private class FormDataMimeSection : HttpWebRequestBuilder.IMimeSection, IDisposable
    {
      private byte[] _encodedFormData;

      public FormDataMimeSection(string mimeBounday, Dictionary<string, string> formData)
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (KeyValuePair<string, string> keyValuePair in formData)
        {
          stringBuilder.Append("--" + mimeBounday + "\r\n");
          stringBuilder.Append("Content-Disposition: form-data; name=\"" + keyValuePair.Key + "\"\r\n");
          stringBuilder.Append("\r\n");
          stringBuilder.Append(keyValuePair.Value + "\r\n");
        }
        this._encodedFormData = Encoding.UTF8.GetBytes(stringBuilder.ToString());
      }

      public long Length => (long) this._encodedFormData.Length;

      public void WriteToStream(Stream requestStream)
      {
        requestStream.Write(this._encodedFormData, 0, this._encodedFormData.Length);
      }

      public void Dispose()
      {
      }
    }

    private class FileMimeSection : HttpWebRequestBuilder.IMimeSection, IDisposable
    {
      private readonly int BUFFER_SIZE = 4096;
      private bool _isDisposed;
      private byte[] _encodedSectionStart;
      private Stream _stream;
      private static readonly byte[] _encodedSectionEnd = Encoding.UTF8.GetBytes("\r\n");

      public FileMimeSection(string mimeBounday, HttpWebRequestBuilder.FileEntry fileData)
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("--" + mimeBounday + "\r\n");
        stringBuilder.Append("Content-Disposition: file; name=\"" + fileData.FieldName + "\"; filename=\"" + fileData.ClientFileName + "\"\r\n");
        stringBuilder.Append("Content-Type: " + fileData.ContentType + "\r\n");
        stringBuilder.Append("\r\n");
        this._encodedSectionStart = Encoding.UTF8.GetBytes(stringBuilder.ToString());
        if (fileData is HttpWebRequestBuilder.StringFileEntry stringFileEntry)
          this._stream = (Stream) new MemoryStream(Encoding.UTF8.GetBytes(stringFileEntry.Contents));
        else
          this._stream = (Stream) new FileStream(fileData.File, FileMode.Open, FileAccess.Read);
      }

      public long Length
      {
        get
        {
          return (long) this._encodedSectionStart.Length + this._stream.Length + (long) HttpWebRequestBuilder.FileMimeSection._encodedSectionEnd.Length;
        }
      }

      public void WriteToStream(Stream requestStream)
      {
        requestStream.Write(this._encodedSectionStart, 0, this._encodedSectionStart.Length);
        byte[] buffer = new byte[this.BUFFER_SIZE];
        this._stream.Seek(0L, SeekOrigin.Begin);
        int count;
        while ((count = this._stream.Read(buffer, 0, buffer.Length)) != 0)
          requestStream.Write(buffer, 0, count);
        requestStream.Write(HttpWebRequestBuilder.FileMimeSection._encodedSectionEnd, 0, HttpWebRequestBuilder.FileMimeSection._encodedSectionEnd.Length);
      }

      public void Dispose()
      {
        this.Dispose(true);
        GC.SuppressFinalize((object) this);
      }

      private void Dispose(bool disposing)
      {
        if (this._isDisposed)
          return;
        if (disposing)
        {
          this._stream.Close();
          this._stream.Dispose();
        }
        this._isDisposed = true;
      }
    }
  }
}
