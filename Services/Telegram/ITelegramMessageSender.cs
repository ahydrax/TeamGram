using System.Threading.Tasks;

namespace TeamGram.Services.Telegram
{
    public interface ITelegramMessageSender
    {
        Task SendMessage(string text);
    }
}
