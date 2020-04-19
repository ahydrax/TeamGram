using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace TeamGram.Logging
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public class PipelineExceptionLogger<TRequest, TResponse> : IRequestExceptionHandler<TRequest, TResponse>
    {
        private readonly ILogger<TRequest> _logger;

        public PipelineExceptionLogger([NotNull] ILogger<TRequest> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Handle(TRequest request, Exception exception, RequestExceptionHandlerState<TResponse> state,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Error occured during processing", request);
            return Task.CompletedTask;
        }
    }
}
