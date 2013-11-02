using Newtonsoft.Json;

namespace ConsoleEvents
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Part
	{
		[JsonProperty]
		public string PartNumber { get; set; }

		[JsonProperty]
		public string Code { get; set; }

		[JsonProperty]
		public int Level { get; set; }

		public Part()
		{
		}

		public string ToJsonString()
		{
			return JsonConvert.SerializeObject(this);
		}
		public static Part Deserialize(string jsonString)
		{
			return JsonConvert.DeserializeObject<Part>(jsonString);
		}
	}
}
