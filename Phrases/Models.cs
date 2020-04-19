using JetBrains.Annotations;
using MongoDB.Bson.Serialization.Attributes;

namespace TeamGram.Phrases
{
    [BsonDiscriminator(Required = true)]
    [BsonKnownTypes(typeof(UserGreetingPhrase))]
    [BsonKnownTypes(typeof(UserFarewellPhrase))]
    [BsonKnownTypes(typeof(ServerIsEmptyPhrase))]
    public abstract class CustomPhrase { }

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [BsonDiscriminator("greeting")]
    public class UserGreetingPhrase : CustomPhrase
    {
        [BsonElement("username")]
        public string? Username { get; set; }

        [BsonElement("template")]
        public string? Template { get; set; }
    }

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [BsonDiscriminator("farewell")]
    public class UserFarewellPhrase : CustomPhrase
    {
        [BsonElement("username")]
        public string? Username { get; set; }

        [BsonElement("template")]
        public string? Template { get; set; }
    }

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [BsonDiscriminator("emptyserver")]
    public class ServerIsEmptyPhrase : CustomPhrase
    {
        [BsonElement("text")]
        public string? Text { get; set; }
    }
}
