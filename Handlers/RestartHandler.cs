using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TeamGram.Telegram;

namespace TeamGram.Handlers
{
    public class RestartHandler : INotificationHandler<RestartRequested>
    {
        public Task Handle(RestartRequested notification, CancellationToken cancellationToken)
        {
            Environment.FailFast("Initiated by telegram");
            return Task.CompletedTask;
        }
    }
}
