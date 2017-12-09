using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace dndmapviewer
{
	public class Map
	{
		[JsonProperty("name")]
		public string name { get; set; }

		[JsonProperty("image_filename")]
		public string image_filename { get; set; }

		[JsonProperty("map_width")]
		public double map_width { get; set; }
	}

	public class Location
	{
		public Location(double[] in_pos) {
			name = "";
			description = "";
			position = in_pos;
			color = new byte[] { 255,255,255 };
			visible = true;
			label = true;
		}

		[JsonProperty("name")]
		public string name { get; set; }

		[JsonProperty("description")]
		public string description { get; set; }

		[JsonProperty("position")]
		public double[] position { get; set; }

		[JsonProperty("color")]
		public byte[] color { get; set; }

		[JsonProperty("visible")]
		public bool visible { get; set; }

		[JsonProperty("label")]
		public bool label { get; set; }
	}

	public class Entity
	{
		public Entity(double[] in_pos)
		{
			name = "";
			position = in_pos;
			color = new byte[] { 255, 255, 255 };
			label = true;
			speed = 30;
			rangeShort = 5;
			rangeLong = 0;
		}

		[JsonProperty("name")]
		public string name { get; set; }

		[JsonProperty("speed")]
		public int speed { get; set; }

		[JsonProperty("rangeShort")]
		public int rangeShort { get; set; }
		
		[JsonProperty("rangeLong")]
		public int rangeLong { get; set; }

		[JsonProperty("position")]
		public double[] position { get; set; }

		[JsonProperty("color")]
		public byte[] color { get; set; }

		[JsonProperty("label")]
		public bool label { get; set; }
	}

	public static class JsonHelper
	{
		public static string FromClass<T>(T data, bool isEmptyToNull = false,
										  JsonSerializerSettings jsonSettings = null)
		{
			string response = string.Empty;

			if (!EqualityComparer<T>.Default.Equals(data, default(T)))
				response = JsonConvert.SerializeObject(data, jsonSettings);

			return isEmptyToNull ? (response == "{}" ? "null" : response) : response;
		}

		public static T ToClass<T>(string data, JsonSerializerSettings jsonSettings = null)
		{
			var response = default(T);

			if (!string.IsNullOrEmpty(data))
				response = jsonSettings == null
					? JsonConvert.DeserializeObject<T>(data)
					: JsonConvert.DeserializeObject<T>(data, jsonSettings);

			return response;
		}
	}
}
