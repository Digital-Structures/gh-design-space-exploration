using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using System.Diagnostics;

namespace StructureEngine.Logging
{

    /// <summary>
    /// Assists in building a Web Request
    /// </summary>
    public class HttpWebRequestBuilder
    {

        #region Data Members

        /// <summary>
        /// The URI to send the request to
        /// </summary>
        private Uri _requestUri = null;

        /// <summary>
        /// The headers to send for the request
        /// </summary>
        private WebHeaderCollection _webHeaderCollection = null;
                
        /// <summary>
        /// The entries for the form data
        /// </summary>
        private Dictionary<string, string> _formData = null;

        /// <summary>
        /// The information needed for each file that will be sent
        /// </summary>
        private List<FileEntry> _files = null;

        #endregion
        
        #region Construction

        /// <summary>
        /// Create the HttpWebRequest from a string
        /// </summary>
        /// <param name="url"></param>
        public HttpWebRequestBuilder(string url)
            : this(new Uri(url)) {}

        /// <summary>
        /// Create the HttpWebRequest from a URI
        /// </summary>
        /// <param name="url"></param>
        public HttpWebRequestBuilder(Uri uri)
        {
            _requestUri = uri;
            _webHeaderCollection = new WebHeaderCollection();
            _formData = new Dictionary<string, string>();
            _files = new List<FileEntry>();
        }

        #endregion

        #region Builder Methods

        public HttpWebResponse SendRequest()
        {
            return SendRequest(new DefaultRequestController());
        }

        /// <summary>
        /// Constructs the request and sends it
        /// </summary>
        /// <returns></returns>
        public HttpWebResponse SendRequest(IRequestController controller)
        {

            // TODO: Ensure that we call this only once

            // Construct the unique boundary string (something that shuoldn't appear in anywhere else
            // in the request stream)
            string boundary = "--" + System.Guid.NewGuid().ToString("N");
            
            // Create the end of the post marker
            byte[] endPostMarkerBytes = Encoding.UTF8.GetBytes("--" + boundary + "--\r\n");
            
            // Create the Mime Sections
            List<IMimeSection> sections = new List<IMimeSection>();
            try
            {

                // Add the form data if needed
                if (_formData.Count > 0)
                {
                    sections.Add(new FormDataMimeSection(boundary, _formData));
                }

                // Add each file
                foreach (FileEntry file in _files)
                {
                    sections.Add(new FileMimeSection(boundary, file));
                }

                // Verify that we have at least one mime section to send
                if (sections.Count == 0)
                {
                    throw new InvalidOperationException("Cannot create request, no mime sections created");
                }

                // Calculate the length of the post
                long contentLength = 0;
                foreach(IMimeSection section in sections)
                {
                    contentLength += section.Length;
                }
                contentLength += endPostMarkerBytes.Length;

                int attemptCount = controller.MaxAttempts;
                if (attemptCount <= 0)
                {
                    throw new Exception("MaxAttempts must be greater than zero");
                }

                for (int i = 0; i < attemptCount; i++)
                {

                    try
                    {

                        // Create the web request
                        HttpWebRequest webRequest = WebRequest.Create(_requestUri) as HttpWebRequest;

                        // Set headers - do this first, some of the methods below override the values
                        webRequest.Headers = _webHeaderCollection;

                        // Set the content type on the web request, along with the boundary that we will use
                        webRequest.Method = "POST";
                        webRequest.ContentType = "multipart/form-data; boundary=" + boundary;

                        // Set the content length (must be done before we get the request stream)
                        webRequest.ContentLength = contentLength;

                        Exception sendException = null;
                        try
                        {

                            // Tell the signal that we are about to send the request
                            controller.OnBeforeSendRequest(webRequest);

                            // Establish connection
                            Stream requestStream = null;
                            try
                            {
                                IAsyncResult res = webRequest.BeginGetRequestStream(null, null);
                                requestStream = webRequest.EndGetRequestStream(res);
                            }
                            catch (Exception ex)
                            {
                                throw ex; // TODO
                            }

                            // Write each request section
                            try
                            {
                                foreach (IMimeSection section in sections)
                                {
                                    // we do not dispose of the section until the end
                                    // this allows us to execute the send of the request more than once if needed
                                    section.WriteToStream(requestStream);
                                }

                                // Write the closing section
                                requestStream.Write(endPostMarkerBytes, 0, endPostMarkerBytes.Length);
                            }
                            finally
                            {
                                // always close the stream
                                requestStream.Close();
                            }

                            // Now that we have finished sending, return the web response
                            // Assume success for now... async is too annoying to deal with
                            webRequest.BeginGetResponse((r) => { return; }, null);

                            /*
                            webRequest.BeginGetResponse(r =>
                            {
                                var reponse = webRequest.EndGetResponse(r);
                            }, null);
                            */
                        }
                        catch (Exception ex)
                        {
                            // save the exception
                            sendException = ex;
                            throw;
                        }
                        finally
                        {
                            controller.OnEndSendRequest(webRequest, sendException);
                        }

                    }
                    catch (Exception ex)
                    {

                        if (i < attemptCount - 1)
                        {

                            if (!controller.OnSendFailure(ex))
                            {
                                // Do not retry, let this exception bubble up to the caller
                                throw;
                            }
                        }
                        else
                        {
                            // max retries
                            throw;
                        }
                        
                    }

                } // retry loop

            }
            finally
            {

                // Ensure that all of the resources have been disposed of
                foreach (IMimeSection section in sections)
                {
                    try
                    {
                        // Ensure it was disposed
                        section.Dispose();
                    }
                    catch (Exception)
                    {
                        // Should we record this? I'm not sure its worth it...
                        // This is really just a safe gaurd to ensure that we don't
                        // leave file handles open.
                    }
                }

            }

            // if we never attempted to send the request
            return null;

        }

        /// <summary>
        /// Access to the headers of the web request
        /// </summary>
        public WebHeaderCollection Headers
        {
            get { return _webHeaderCollection; }
        }

        /// <summary>
        /// Returns the collection of Form Data.
        /// </summary>
        public Dictionary<string, string> FormData
        {
            get { return _formData; }
        }

        public void AddStringFile(string fieldName, string fileName, string contents)
        {
            var stringEntry = new StringFileEntry(fieldName, fileName, contents);
            _files.Add(stringEntry);
        }

        /// <summary>
        /// Appends the specified file to the Request
        /// </summary>
        /// <param name="fieldName">The name that will be associated with the file in the post</param>
        /// <param name="file">The full path of the file to post</param>
        public void AddFile(string fieldName, string file)
        {
            FileEntry fileEntry = new FileEntry(fieldName, file);
            _files.Add(fileEntry);
        }

        /// <summary>
        /// Appends the specified file to the Request
        /// </summary>
        /// <param name="fieldName">The name that will be associated with the file in the post</param>
        /// <param name="file">The full path of the file to post</param>
        public void AddFile(string fieldName, string file, string clientFileName)
        {
            FileEntry fileEntry = new FileEntry(fieldName, file, clientFileName);
            _files.Add(fileEntry);
        }

        public void AddFile(string fieldName, string file, string clientFileName, string contentType)
        {
            FileEntry fileEntry = new FileEntry(fieldName, file, clientFileName, contentType);
            _files.Add(fileEntry);
        }

        #endregion

        #region Helper Classes

        private class StringFileEntry : FileEntry
        {
            public string Contents  { get; private set; }

            public StringFileEntry(string fieldName, string fileName, string content)
                : base(fieldName, null, fileName)
            {
                this.Contents = content;
            }
        }

        /// <summary>
        /// Represents a single file to post
        /// </summary>
        private class FileEntry 
        {
            #region Properties

            private const string DEFAULT_CONTENT_TYPE = "application/octet-stream";

            /// <summary>
            /// The file to post
            /// </summary>
            public string File { get; private set; }

            /// <summary>
            /// The field name to use in the post (how it will be accessed by the receiver)
            /// </summary>
            public string FieldName { get; private set; }

            /// <summary>
            /// The client file name of the client
            /// </summary>
            public string ClientFileName { get; private set; }

            /// <summary>
            /// The mime type for the file
            /// </summary>
            public string ContentType { get; private set; }

            #endregion

            #region Construction

            /// <summary>
            /// Creates a new file entry
            /// </summary>
            /// <param name="fieldName"></param>
            /// <param name="file"></param>
            public FileEntry(string fieldName, string file)
            {
                FieldName = fieldName;
                File = file;
                ClientFileName = Path.GetFileName(file);
                ContentType = DEFAULT_CONTENT_TYPE;
            }

            /// <summary>
            /// Creates a new file entry
            /// </summary>
            /// <param name="fieldName"></param>
            /// <param name="file"></param>
            public FileEntry(string fieldName, string file, string clientFilename)
            {
                FieldName = fieldName;
                File = file;
                ClientFileName = clientFilename;
                ContentType = DEFAULT_CONTENT_TYPE;
            }

            public FileEntry(string fieldName, string file, string clientFilename, string contentType)
            {
                FieldName = fieldName;
                File = file;
                ClientFileName = clientFilename;
                ContentType = (string.IsNullOrEmpty(contentType) ? DEFAULT_CONTENT_TYPE : contentType);
            }

            #endregion

        }

        /// <summary>
        /// The interface that all mime sectios must implement
        /// </summary>
        private interface IMimeSection : IDisposable
        {

            /// <summary>
            /// The total length of the Mime section in bytes
            /// </summary>
            long Length { get; }

            /// <summary>
            /// Writes the Mime Section to the given stream
            /// </summary>
            void WriteToStream(Stream requestStream);

        }

        /// <summary>
        /// Represents a mime section that contains Form Data
        /// </summary>
        private class FormDataMimeSection : IMimeSection
        {

            private byte[] _encodedFormData = null;

            public FormDataMimeSection(string mimeBounday, Dictionary<string, string> formData)
            {
                // Create the StringBuilder
                StringBuilder formDataSection = new StringBuilder();

                // Add each form data
                foreach (var kvp in formData)
                {
                    formDataSection.Append("--" + mimeBounday + "\r\n");
                    formDataSection.Append("Content-Disposition: form-data; name=\"" + kvp.Key + "\"\r\n");
                    formDataSection.Append("\r\n");
                    formDataSection.Append(kvp.Value + "\r\n");
                }

                // Convert to bytes
                _encodedFormData = Encoding.UTF8.GetBytes(formDataSection.ToString());

            }

            #region IMimeSection Members

            /// <summary>
            /// Returns the number of bytes for this Mime Section
            /// </summary>
            public long Length
            {
                get { return _encodedFormData.Length; }
            }

            /// <summary>
            /// Writes the mime section to the stream
            /// </summary>
            public void WriteToStream(Stream requestStream)
            {
                requestStream.Write(_encodedFormData, 0, _encodedFormData.Length);
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                // nothing to do, the bytes will get release when the class goes
                // out of scope.
            }

            #endregion
        }

        /// <summary>
        /// Represents a mime section that is a posted file
        /// </summary>
        private class FileMimeSection : IMimeSection
        {

            #region Constants

            /// <summary>
            /// The buffer size used to read from the files and transfer to over the request
            /// </summary>
            private readonly int BUFFER_SIZE = 4096;

            #endregion

            #region Data Members

            /// <summary>
            /// Boolean to indicate if disposed has been called
            /// </summary>
            private bool _isDisposed = false;

            /// <summary>
            /// The bytes that start the mime section for this file. File contents will be
            /// emitted after this
            /// </summary>
            private byte[] _encodedSectionStart = null;

            /// <summary>
            /// The file stream for the file being posted
            /// </summary>
            private Stream _stream = null;

            /// <summary>
            /// The bytes representing the end of the section (same for all files)
            /// </summary>
            private static readonly byte[] _encodedSectionEnd = Encoding.UTF8.GetBytes("\r\n");

            #endregion

            /// <summary>
            /// Creates a new mime section for posting the given file
            /// </summary>
            /// <param name="mimeBounday"></param>
            /// <param name="fileData"></param>
            public FileMimeSection(string mimeBounday, FileEntry fileData)
            {

                // Create the start of the mime section
                StringBuilder sb = new StringBuilder();
                sb.Append("--" + mimeBounday + "\r\n");
                // TODO: should this JUST be the file name, or the full path?
                sb.Append("Content-Disposition: file; name=\"" + fileData.FieldName + "\"; filename=\"" + fileData.ClientFileName + "\"\r\n");
                sb.Append("Content-Type: " + fileData.ContentType + "\r\n");
                sb.Append("\r\n");
                _encodedSectionStart = Encoding.UTF8.GetBytes(sb.ToString());
                
                // Open the file
                var stringEntry = fileData as StringFileEntry;
                if (stringEntry != null)
                {
                    _stream = new MemoryStream(Encoding.UTF8.GetBytes(stringEntry.Contents));
                }
                else
                {
                    _stream = new FileStream(fileData.File, FileMode.Open, FileAccess.Read);
                }
            }

            #region IMimeSection Members

            public long Length
            {
                get 
                {
                    return _encodedSectionStart.Length +
                           _stream.Length +
                           _encodedSectionEnd.Length;
                }
            }

            public void WriteToStream(Stream requestStream)
            {
                // Write the section start
                requestStream.Write(_encodedSectionStart, 0, _encodedSectionStart.Length);

                // Transfer the file in chunks
                int bytesRead = 0;
                byte[] buffer = new byte[BUFFER_SIZE];
                // Seek to the beginning of the file (since we could call this method twice in a row)
                _stream.Seek(0, SeekOrigin.Begin);
                while ((bytesRead = _stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    requestStream.Write(buffer, 0, bytesRead);
                }

                // Write the section end
                requestStream.Write(_encodedSectionEnd, 0, _encodedSectionEnd.Length);
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (!_isDisposed)
                {
                    if (disposing)
                    { // release managed resources
                        // Close and dipose of the file stream
                        _stream.Close();
                        _stream.Dispose();
                    }
                   // indicate that we disposed of the class
                    _isDisposed = true;
                }
            }

            #endregion
        }

        #endregion

    }

    public interface IRequestController
    {

        /// <summary>
        /// The maximum number of times to try sending the webrequest.
        /// This should always be greater than 1.
        /// </summary>
        int MaxAttempts { get; }

        /// <summary>
        /// Called when a send failures, and we could retry (we have not reached the max attempt limit). 
        /// This function determine if we should retry or give up (usually based on the exception).
        /// If we have reached the attempt limit, this will not be called, and instead the exception
        /// will just propagate up the stack.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns>True if we should retry the request, or false if not</returns>
        bool OnSendFailure(Exception ex);

        /// <summary>
        /// Called before each attempt to send the request.
        /// </summary>
        /// <param name="request"></param>
        void OnBeforeSendRequest(HttpWebRequest request);

        /// <summary>
        /// Called at the end of each attempt to send the request.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="sendException"></param>
        void OnEndSendRequest(HttpWebRequest request, Exception sendException);

    }

    public class DefaultRequestController : IRequestController
    {

        /// <summary>
        /// The maximum number of times that we will try to send the request. Should
        /// always be greater than 0;
        /// </summary>
        public virtual int MaxAttempts { get { return 1; } }

        /// <summary>
        /// Called when a send fails, and we still have retry attempts available.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns>Returns true if the request should be retried</returns>
        public virtual bool OnSendFailure(Exception ex)
        {

            bool retry = false;

            System.Net.WebException webEx = ex as System.Net.WebException;
            if (webEx != null)
            {

                // retry allowed
                retry = true;

            }
            else
            {
                System.IO.IOException ioEx = ex as System.IO.IOException;
                if (ioEx != null)
                {
                    // retry allowed
                    retry = true;
                }
                else
                {

                    System.Net.Sockets.SocketException socketEx = ex as System.Net.Sockets.SocketException;
                    if (socketEx != null)
                    {
                        // retry allowed
                        retry = true;
                    }
                }
            }

            if (!retry)
            {
                // Don't retry
                return false;
            }

            // Record and retry
            RecordRetryError(ex);
            return true;

        }

        public virtual void RecordRetryError(Exception ex)
        {
            // Don't log anything by default
        }

        public virtual void OnBeforeSendRequest(HttpWebRequest request) { }

        public virtual void OnEndSendRequest(HttpWebRequest request, Exception sendException) { }

    }

}

