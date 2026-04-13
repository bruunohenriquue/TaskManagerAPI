using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Exceptions;

namespace TaskManager.Api.Handlers;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is TaskNotFoundException notFound)
        {
            _logger.LogWarning("Tarefa {TaskId} não encontrada", notFound.TaskId);
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            await httpContext.Response.WriteAsJsonAsync(
                new ProblemDetails
                {
                    Title = "Recurso não encontrado",
                    Detail = notFound.Message,
                    Status = StatusCodes.Status404NotFound
                },
                cancellationToken);
            return true;
        }

        _logger.LogError(exception, "Falha não tratada na requisição");
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Title = "Erro interno",
                Detail = "Não foi possível concluir a operação.",
                Status = StatusCodes.Status500InternalServerError
            },
            cancellationToken);
        return true;
    }
}
