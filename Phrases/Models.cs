using MongoDB.Bson.Serialization.Attributes;

namespace TeamGram.Phrases
{
    [BsonDiscriminator("type", Required = true)]
    [BsonKnownTypes(typeof(UserGreetingPhrase))]
    [BsonKnownTypes(typeof(UserFarewellPhrase))]
    [BsonKnownTypes(typeof(ServerIsEmptyPhrase))]
    public abstract class CustomPhrase { }

    public class UserGreetingPhrase : CustomPhrase
    {
        [BsonElement("username")]
        public string Username { get; }

        [BsonElement("template")]
        public string Template { get; }

        public UserGreetingPhrase(string username, string template)
        {
            Username = username;
            Template = template;
        }
    }

    public class UserFarewellPhrase : CustomPhrase
    {
        [BsonElement("username")]
        public string Username { get; }

        [BsonElement("template")]
        public string Template { get; }

        public UserFarewellPhrase(string username, string template)
        {
            Username = username;
            Template = template;
        }
    }

    public class ServerIsEmptyPhrase : CustomPhrase
    {
        [BsonElement("text")]
        public string Text { get; }

        public ServerIsEmptyPhrase(string text)
        {
            Text = text;
        }
    }
}
