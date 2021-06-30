using DoberDogBot.Application.Attributes;
using DoberDogBot.Domain.Extensions;
using DoberDogBot.Infrastructure.AppDb;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DoberDogBot.Application.Behavior
{
    public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> _logger;
        private readonly AppDbContext _dbContext;

        public TransactionBehaviour(ILogger<TransactionBehaviour<TRequest, TResponse>> logger,
            AppDbContext dbContext)
        {
            _logger = logger ?? throw new ArgumentException(nameof(ILogger));
            _dbContext = dbContext ?? throw new ArgumentException(nameof(AppDbContext));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = default(TResponse);
            var type = request.GetType();
            var typeName = type.GetGenericTypeName();

            try
            {
                if (_dbContext.HasActiveTransaction || Attribute.GetCustomAttributes(type).Any(x => x is NonTransactionalAttribute))
                {
                    return await next();
                }

                var strategy = _dbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    Guid transactionId;

                    using (var transaction = await _dbContext.BeginTransactionAsync())
                    using (LogContext.PushProperty("TransactionContext", transaction.TransactionId))
                    {
                        _logger.LogInformation("----- Begin transaction {TransactionId} for {CommandName}", transaction.TransactionId, typeName);

                        response = await next();

                        _logger.LogInformation("----- Commit transaction {TransactionId} for {CommandName}", transaction.TransactionId, typeName);

                        await _dbContext.CommitTransactionAsync(transaction);

                        transactionId = transaction.TransactionId;
                    }
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Handling transaction for {CommandName}", typeName);

                throw;
            }
        }
    }
}