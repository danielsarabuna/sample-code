using Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Gameplay
{
    [System.Serializable]
    public class EquipmentCollection
    {
        public readonly EquipmentModel[] Models;

        public EquipmentCollection(EquipmentModel[] models)
        {
            Models = models;
        }

        public static EquipmentCollection FromJson(string json) =>
            JsonConvert.DeserializeObject<EquipmentCollection>(json, Converter.Settings);

        public string ToJson() => JsonConvert.SerializeObject(this, Converter.Settings);

        private static class Converter
        {
            public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                SerializationBinder = new DefaultSerializationBinder(),
            };
        }
    }
}