using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace ConsoleEvents
{
	[JsonObject(MemberSerialization.OptIn)]
	public class EventLog
	{
		[JsonProperty(PropertyName="eventCode")]
		public string EventCode {get;set;}

		[JsonProperty(PropertyName = "msgText")]
		public string MessageText { get; set; }

		[JsonProperty(PropertyName = "timeStamp")]
		public DateTime TimeStamp { get; set; }

		public string ToJsonString()
		{
			return JsonConvert.SerializeObject(this);
		}
		public static EventLog Deserialize(string jsonString)
		{
			return JsonConvert.DeserializeObject<EventLog>(jsonString);
		}
	}
}
