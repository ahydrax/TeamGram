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
        public string Username { get; private set; }

        [BsonElement("template")]
        public string Template { get; private set; }

        public UserGreetingPhrase(string username, string template)
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

        public UserFarewellPhrase(string username, string template)
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

        public ServerIsEmptyPhrase(string text)
        {
            Text = text;
        }
    }
}
