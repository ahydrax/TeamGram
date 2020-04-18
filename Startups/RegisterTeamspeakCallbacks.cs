using System;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace TeamGram.Startups
{
    public class RegisterTeamspeakCallbacks : IStartupFilter
    {
        private readonly IMediator _mediator;

        public RegisterTeamspeakCallbacks(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                next(builder);
            };
        }
    }
}
