using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Logging;

namespace TeamGram.Logging
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public class LoggingMediator : Mediator
    {
        private readonly ILogger<Mediator> _logger;

        public LoggingMediator(ServiceFactory serviceFactory, [NotNull] ILogger<Mediator> logger) : base(serviceFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override Task PublishCore(IEnumerable<Func<INotification, CancellationToken, Task>> allHandlers,
            INotification notification, CancellationToken cancellationToken)
        {
            var exceptionLoggingHandlers = allHandlers.Select(x =>
                new Func<INotification, CancellationToken, Task>(
                    async (n, ct) =>
                    {
                        try
                        {
                            await x(n, ct);
                        }
                        catch (Exception e)
                        {
                            var handler = x.Method.ReflectedType;
                            _logger.LogError(e, $"Exception occured during processing {n} in {handler}",
                                n,
                                handler!.FullName);
                            throw;
                        }
                    })).ToList();

            return base.PublishCore(exceptionLoggingHandlers, notification, cancellationToken);
        }
    }
}
