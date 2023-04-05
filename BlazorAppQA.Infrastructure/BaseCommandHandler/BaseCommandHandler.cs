using System;
using System.Threading.Tasks;
using BlazorAppQA.Infrastructure.Common;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorAppQA.Infrastructure.BaseCommandHandler
{
    public abstract class BaseCommandHandler<TCommand> : IDisposable
    {
        protected readonly IServiceScopeFactory _serviceScopeFactory;
        protected readonly IValidator<TCommand> _validator;

        protected BaseCommandHandler(IServiceProvider provider)
        {
            _serviceScopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
            _validator = provider.GetService<IValidator<TCommand>>();
        }
        protected abstract Task<dynamic> ExecuteAsync(TCommand command);
        public async Task<dynamic> HandleAsync(TCommand command)
        {
            var validationResult = _validator?.Validate(command);
            if (validationResult != null && !validationResult.IsValid)
            {
                throw new CustomException("The received data model is invalid.");
            }

            return await ExecuteAsync(command);
        }
        public abstract void Dispose();
    }
}
