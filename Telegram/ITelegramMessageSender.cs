using System.Threading;
using System.Threading.Tasks;

namespace TeamGram.Telegram
{
    public interface ITelegramMessageSender
    {
        Task SendMessage(string text, CancellationToken cancellationToken = default);
    }
}
