using Model.Actor;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Gameplay
{
    [System.Serializable]
    public class ActorCollection
    {
        public readonly ActorModel[] Models;

        public ActorCollection(ActorModel[] models)
        {
            Models = models;
        }

        public static ActorCollection FromJson(string json) =>
            JsonConvert.DeserializeObject<ActorCollection>(json, Converter.Settings);

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