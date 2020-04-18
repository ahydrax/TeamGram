using MediatR;

namespace TeamGram.Telegram
{
    public class UserListAsked : INotification { }

    public class CredentialsAsked : INotification { }

    public class NewTextMessage : INotification
    {
        public string MessageText { get; }

        public NewTextMessage(string messageText)
        {
            MessageText = messageText;
        }
    }
}
