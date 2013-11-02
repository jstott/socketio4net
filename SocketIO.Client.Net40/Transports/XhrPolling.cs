using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

namespace SocketIOClient
{
    public class XhrPolling : ITransport
    {
        private Uri _uri
        {
            get
            {
                return new Uri(Handshake.Uri, string.Format("socket.io/1/xhr-polling/{0}/", Handshake.SID));
            }
        }
        private readonly HttpClient _http = new HttpClient();
        private Task msgTask;
        public event EventHandler Opened;
        public event EventHandler Closed;
        public event EventHandler<ErrorEventArgs> Error;
        public event EventHandler<TransportReceivedEventArgs> MessageReceived;

        public IOHandshake Handshake { get; set; }
        public TransportType TransportType
        {
            get { return TransportType.XhrPolling; }
        }
        public XhrPolling(IOHandshake handshake)
        {
            this.Handshake = handshake;
        }

        private Uri Uri
        {
            get
            {
                //return new UriBuilder((_uri)
                //       {
                //           Query = string.Format("t={0}", Environment.TickCount)
                //       }.Uri;
                return new UriBuilder(new Uri(Handshake.Uri, string.Format("socket.io/1/xhr-polling/{0}/", Handshake.SID)))
                       {
                           Query = string.Format("t={0}", Environment.TickCount)
                       }.Uri;
            }
        }

        private void OnError(Exception ex)
        {
            if (Error != null)
            {
                Error(this, new ErrorEventArgs(ex.Message));
            }
        }

        public WebSocketState State { get; private set; }
        public bool EnableAutoSendPing { get; set; }

        public Task<bool> OpenAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            try
            {
                this.State = WebSocketState.Connecting;

                msgTask = new Task(() => MessageLoop(tcs), TaskCreationOptions.LongRunning);
                msgTask.Start();
            }
            catch (HttpRequestException ex)
            {
                this.OnError(ex);
            }
            return tcs.Task;
        }
       
        private async void MessageLoop(TaskCompletionSource<bool> tcs)
        {
            var pollTask = _http.GetAsync(Uri);
            

            while (pollTask != null)
            {
                try
                {
                    var response = await pollTask;
                    if (this.State == WebSocketState.Connecting)
                    {
                        this.State = WebSocketState.Open;
                        tcs.SetResult(true); // return openAsync task
                        if (this.Opened != null) { this.Opened(this, EventArgs.Empty); }
                    }

                    response.EnsureSuccessStatusCode(); // Checks for OK 200 result
                    var received = await response.Content.ReadAsStringAsync();

                    var frames = received.Split('\ufffd').Skip(1).Where((s, i) => i % 2 != 0).ToList();

                    if (this.MessageReceived != null)
                    {
                        if (frames.Count > 0)
                        {
                            frames.ForEach(frame => MessageReceived(this, new TransportReceivedEventArgs(frame)));
                        }
                        else
                        {
                            this.MessageReceived(this, new TransportReceivedEventArgs(received));
                        }
                    }

                    pollTask = this.State != WebSocketState.Open ? null : _http.GetAsync(Uri); // State is changed by Close()
                }
                catch (Exception ex)
                {
                    if (this.State == WebSocketState.Connecting)
                        tcs.SetException(ex);
                    this.OnError(ex);
                }
            }
        }

        

        public async void Send(string message)
        {
            try
            {
                (await _http.PostAsync(Uri, new StringContent(message))).EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                this.OnError(ex);
            }
        }


        public async void Close()
        {
            this.State = WebSocketState.Closing;

            try
            {
                (await _http.PostAsync(new UriBuilder(_uri) { Query = "disconnect" }.Uri, null)).EnsureSuccessStatusCode();

            }

            catch (HttpRequestException ex)
            {
                this.OnError(ex);
            }

            this.State = WebSocketState.Closed;
            if (this.Closed != null) { this.Closed(this, EventArgs.Empty); }
        }
        public async Task<bool> Reconnect()
        {
            this.Handshake.ResetConnection();
            return await OpenAsync();
        }
    }
}
