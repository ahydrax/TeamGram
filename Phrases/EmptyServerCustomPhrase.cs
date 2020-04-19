using MongoDB.Bson.Serialization.Attributes;

namespace TeamGram.Phrases
{
    public class EmptyServerCustomPhrase
    {
        public string Phrase { get; private set; }

        public EmptyServerCustomPhrase(string phrase)
        {
            Phrase = phrase;
        }
    }
}
