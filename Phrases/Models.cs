using JetBrains.Annotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace TeamGram.Phrases
{
    [BsonDiscriminator(Required = true)]
    [BsonIgnoreExtraElements(true, Inherited = true)]
    [BsonKnownTypes(typeof(UserGreetingPhrase))]
    [BsonKnownTypes(typeof(UserFarewellPhrase))]
    [BsonKnownTypes(typeof(ServerIsEmptyPhrase))]
    public abstract class CustomPhrase
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; private set; }

        protected CustomPhrase(string id)
        {
            Id = id;
        }
    }

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [BsonDiscriminator("greeting")]
    public class UserGreetingPhrase : CustomPhrase
    {
        [BsonElement("username")]
        public string Username { get; private set; }

        [BsonElement("template")]
        public string Template { get; private set; }

        [BsonConstructor]
        public UserGreetingPhrase(string id, string username, string template) : base(id)
        {
            Username = username;
            Template = template;
        }
    }

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [BsonDiscriminator("farewell")]
    public class UserFarewellPhrase : CustomPhrase
    {
        [BsonElement("username")]
        public string Username { get; private set; }

        [BsonElement("template")]
        public string Template { get; private set; }

        [BsonConstructor]
        public UserFarewellPhrase(string id, string username, string template) : base(id)
        {
            Username = username;
            Template = template;
        }
    }

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [BsonDiscriminator("emptyserver")]
    public class ServerIsEmptyPhrase : CustomPhrase
    {
        [BsonElement("text")]
        public string Text { get; private set; }

        [BsonConstructor]
        public ServerIsEmptyPhrase(string id, string text) : base(id)
        {
            Text = text;
        }
    }
}
