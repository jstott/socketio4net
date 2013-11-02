using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using SimpleJson.Reflection;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;

namespace SocketIOClient
{
	public class IOHandshake
	{
	    public Uri Uri { get; set; }
		public string SID { get; set; }
		public int HeartbeatTimeout { get; set; }
		public string ErrorMessage { get; set; }
		public bool HadError
		{
			get { return !string.IsNullOrWhiteSpace(this.ErrorMessage); }

		}

        /// <summary>
		/// The HearbeatInterval will be approximately 20% faster than the Socket.IO service indicated was required
		/// </summary>
		public TimeSpan HeartbeatInterval
		{
			get
			{
				return new TimeSpan(0, 0, HeartbeatTimeout);
			}
		}
		public int ConnectionTimeout { get; set; }

        /// <summary>
        /// List of transports as returned from socket.io handshake
        /// </summary>
		public List<string> Transports = new List<string>(); //websocket,htmlfile,xhr-polling,jsonp-polling
		/// <summary>
		/// Union of transports defined from socket.io server handshake and the transports supported by client
		/// </summary>
        public List<TransportType> AvailableTransports
		{
			get
            {   
                //var xx = (from ioType in Transports
                //              let lcName = ioType.Replace("-",string.Empty)
                //              where Enum.GetNames(typeof (TransportType)).Contains(lcName, StringComparer.CurrentCultureIgnoreCase)
                //              select (TransportType)Enum.Parse(typeof(TransportType),lcName,true)).ToList();
                
                List<TransportType> types = new List<TransportType>();
			    foreach (string transport in Transports)
			    {
			        TransportType type;
			        if (Enum.TryParse(transport.Replace("-", string.Empty), true, out type))
                        types.Add(type);
			    }
                //return (from name in Enum.GetNames(typeof (TransportType))
                //    let lcName = name.Replace("-", string.Empty)
                //        where Transports.Contains(lcName, StringComparer.CurrentCultureIgnoreCase)
                //    select (TransportType) Enum.Parse(typeof (TransportType), name, true)).ToList();
			    return types;

                
			}
		}
		public NameValueCollection Headers;

		public IOHandshake(Uri uri):this(uri, null)
		{
		}
        public IOHandshake(Uri uri, NameValueCollection headers)
        {
            this.Uri = uri;
            this.Headers = headers ?? new NameValueCollection();
        }
		public void ResetConnection()
		{
			this.SID = string.Empty;
			this.ErrorMessage = string.Empty;
		}

	    public async Task RequestHandshake()
        {
            string value = string.Empty;
            string errorText = string.Empty;

            using (WebClient client = new WebClient())
            {
                if (this.Headers.Count > 0)
                    client.Headers.Add(this.Headers);
                try
                {
                    value = await client.DownloadStringTaskAsync(string.Format("{0}://{1}:{2}/socket.io/1/{3}", Uri.Scheme, Uri.Host, Uri.Port, Uri.Query)); // #5 tkiley: The uri.Query is available in socket.io's handshakeData object during authorization
                    // 13052140081337757257:15:25:websocket,htmlfile,xhr-polling,jsonp-polling
                    if (string.IsNullOrEmpty(value))
                        errorText = "Did not receive handshake from server";
                }
                #region Catch Exceptions
                catch (WebException webEx)
                {
                    Trace.WriteLine(string.Format("Handshake threw an exception...{0}", webEx.Message));
                    switch (webEx.Status)
                    {
                        case WebExceptionStatus.ConnectFailure:
                            errorText = string.Format("Unable to contact the server: {0}", webEx.Status);
                            break;
                        case WebExceptionStatus.NameResolutionFailure:
                            errorText = string.Format("Unable to resolve address: {0}", webEx.Status);
                            break;
                        case WebExceptionStatus.ProtocolError:
                            var resp = webEx.Response as HttpWebResponse;//((System.Net.HttpWebResponse)(webEx.Response))
                            if (resp != null)
                            {
                                switch (resp.StatusCode)
                                {
                                    case HttpStatusCode.Forbidden:
                                        errorText = "Socket.IO Handshake Authorization failed";
                                        break;
                                    default:
                                        errorText = string.Format("Handshake response status code: {0}", resp.StatusCode);
                                        break;
                                }
                            }
                            else
                                errorText = string.Format("Error getting handshake from Socket.IO host instance: {0}", webEx.Message);
                            break;
                        default:
                            errorText = string.Format("Handshake threw an exception...{0}", webEx.Message);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    errorText = string.Format("Error getting handshake from Socket.IO host instance: {0}", ex.Message);
                    //this.OnErrorEvent(this, new ErrorEventArgs(errMsg));
                }
                #endregion
            }
            if (string.IsNullOrEmpty(errorText))
                this.UpdateFromSocketIOResponse(value);
            else
                this.ErrorMessage = errorText;
        }
		/// <summary>
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public void UpdateFromSocketIOResponse(string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				this.ErrorMessage = string.Empty;
				string[] items = value.Split(new char[] { ':' });
				if (items.Count() == 4)
				{
					int hb = 0;
					int ct = 0;
					this.SID = items[0];

					if (int.TryParse(items[1], out hb))
					{
						var pct = (int)(hb * .75);  // setup client time to occur 25% faster than needed
						this.HeartbeatTimeout = pct;
					}
					if (int.TryParse(items[2], out ct))
					{
						this.ConnectionTimeout = ct;
					}
					this.Transports.AddRange(items[3].Split(new char[] { ',' }));
				}
			}
		}

	}
}
