using MediatR;

namespace TeamGram.Teamspeak
{
    public class UserJoined : INotification
    {
        public string Username { get; }

        public UserJoined(string username)
        {
            Username = username;
        }
    }

    public class UserLeft : INotification
    {
        public string Username { get; }

        public UserLeft(string username)
        {
            Username = username;
        }
    }
}
