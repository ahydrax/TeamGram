using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TeamGram.Telegram;

namespace TeamGram.Handlers
{
    public class RestartHandler : INotificationHandler<RestartRequested>
    {
        public async Task Handle(RestartRequested notification, CancellationToken cancellationToken)
        {
            await Task.Delay(5, cancellationToken);
            Environment.FailFast("Initiated by telegram");
        }
    }
}
