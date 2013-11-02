using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketIOClient.Messages
{
    public class ErrorMessage : Message
    {
		private static string[] reasons = new string[] { "transport not supported", "client not handshaken", "unauthorized" };
		private static string[] advice =  new string[] {"reconnect"};

		private string reasonCode;
		public string Reason
		{
			get
			{
				int rdx = 0;
				if (!string.IsNullOrEmpty(reasonCode) && int.TryParse(reasonCode, out rdx))
				{
					if (rdx < reasons.Length)
						return reasons[rdx];
				}
				return string.Empty;
			}
		}

		private string adviceCode;
		public string Advice
		{
			get
			{
				// attempt to resolve advice code
				int idx = 0;
				if (!string.IsNullOrEmpty(adviceCode) && int.TryParse(adviceCode, out idx))
				{
					if (idx < advice.Length)
						return advice[idx];
				}
				return string.Empty;
			}
		}

		public override string Event
		{
			get { return "error"; }
		}

		public ErrorMessage()
        {
            this.MessageType = SocketIOMessageTypes.Error;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rawMessage">'7::' [endpoint] ':' [reason] '+' [advice]</param>
		/// <returns>ErrorMessage</returns>
		public static ErrorMessage Deserialize(string rawMessage)
		{
			ErrorMessage errMsg = new ErrorMessage();
			string[] args = rawMessage.Split(':');
			if (args.Length == 4)
			{
				errMsg.Endpoint = args[2];
				errMsg.MessageText = args[3];

				string[] complex = args[3].Split(new char[] { '+' });
				if (complex.Length > 0)
				{
					errMsg.reasonCode = complex[0];
					if (complex.Length > 1)
						errMsg.adviceCode = complex[1];
				}
			}
			return errMsg;
		}
    }
}
