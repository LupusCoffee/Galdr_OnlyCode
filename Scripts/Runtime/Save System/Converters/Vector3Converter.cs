using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace SaveSystem.Converters
{
	public class Vector3Converter : JsonConverter<Vector3>
	{
		public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
		{
			writer.WriteStartObject();
			writer.WritePropertyName("x");
			writer.WriteValue(value.x);
			writer.WritePropertyName("y");
			writer.WriteValue(value.y);
			writer.WritePropertyName("z");
			writer.WriteValue(value.z);
			writer.WriteEndObject();
		}

		/// <inheritdoc />
		public override Vector3 ReadJson(JsonReader reader, 
			Type objectType, 
			Vector3 existingValue,
			bool hasExistingValue,
			JsonSerializer serializer)
		{
			JObject obj = JObject.Load(reader);
			return new Vector3(
				obj.Value<float>("x"), 
				obj.Value<float>("y"),
				obj.Value<float>("z")
			);
		}
	}
}