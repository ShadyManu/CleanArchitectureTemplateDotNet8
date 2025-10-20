using Application.Common.Interfaces.CQRS;
using Application.Common.Result;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviours;

/// <summary>
/// Everytime a Command/Query handler is invoked, these classes will catch unhandled exceptions
/// Log the exception and rethrow it
/// </summary>
internal static class UnhandledExceptionDecorator
{
    internal sealed class QueryHandlerUnhandledException<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> innerHandler,
        ILogger<TQuery> logger)
            : IQueryHandler<TQuery, TResponse>
            where TQuery : IQuery<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
        {
            try
            {
                return await innerHandler.Handle(query, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Unhandled exception occurred while processing query of type {QueryType}",
                    typeof(TQuery).Name);
                
                throw;
            }
        }
    }
    
    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        ILogger<TCommand> logger)
            : ICommandHandler<TCommand, TResponse>
            where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            try
            {
                return await innerHandler.Handle(command, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Unhandled exception occurred while processing query of type {QueryType}",
                    typeof(TCommand).Name);
                
                throw;
            }
        }
    }
}
