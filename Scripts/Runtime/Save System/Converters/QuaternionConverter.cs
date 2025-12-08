// Created by Martin M
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace SaveSystem.Converters
{
	public class QuaternionConverter : JsonConverter<Quaternion>
	{
		public override void WriteJson(JsonWriter writer, 
			Quaternion value, 
			JsonSerializer serializer)
		{
			writer.WriteStartObject();
			writer.WritePropertyName("x");
			writer.WriteValue(value.x);
			writer.WritePropertyName("y");
			writer.WriteValue(value.y);
			writer.WritePropertyName("z");
			writer.WriteValue(value.z);
			writer.WritePropertyName("w");
			writer.WriteValue(value.w);
			writer.WriteEndObject();
		}

		/// <inheritdoc />
		public override Quaternion ReadJson(JsonReader reader, 
			Type objectType, 
			Quaternion existingValue,
			bool hasExistingValue,
			JsonSerializer serializer)
		{
			JObject obj = JObject.Load(reader);
			return new Quaternion(
				obj.Value<float>("x"), 
				obj.Value<float>("y"),
				obj.Value<float>("z"),
				obj.Value<float>("w")
			);
		}
	}
}