using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace UADAPI.Converters
{
    class IAnimeSeriesManagerDeserializer : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return ApiHelpper.ManagerTypes.Any(f => f.FullName == objectType.FullName);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonSetting = new JsonSerializerSettings()
            {
                Error = new EventHandler<ErrorEventArgs>((s, e) => e.ErrorContext.Handled = true),
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
            };

            var root = ((JObject)serializer.Deserialize(reader)).Children();
            var mod = root.FirstOrDefault(f => f.Path == "ModInfo").Children().First().Children().FirstOrDefault(f => f.Path == "ModInfo.ModTypeString");
            var typeName = mod.Value<JProperty>().Value.ToString();
            var animeInfoStr = root.FirstOrDefault(f => f.Path == "AttachedAnimeSeriesInfo").Children().First().ToString();
            IAnimeSeriesManager manager = ApiHelpper.CreateAnimeSeriesManagerObjectByClassName(typeName);
            manager.AttachedAnimeSeriesInfo = JsonConvert.DeserializeObject<AnimeSeriesInfo>(animeInfoStr, jsonSetting);

            return manager;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
