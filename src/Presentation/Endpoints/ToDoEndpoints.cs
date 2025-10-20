using Application.Common.Interfaces.CQRS;
using Application.Dtos.ToDo.Request;
using Application.Dtos.ToDo.Response;
using Application.Features.ToDo.Commands.Create;
using Application.Features.ToDo.Commands.Delete;
using Application.Features.ToDo.Commands.Update;
using Application.Features.ToDo.Queries.Get;
using Application.Features.ToDo.Queries.GetAll;
using Carter.OpenApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Presentation.Extensions;

namespace Presentation.Endpoints;

public class ToDoEndpoints() : ApiModule("/todo")
{
    private const string EndpointTag = "ToDo";
    
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        // GET
        app.MapGet("/", async (
                    IQueryHandler<GetAllToDosQuery, List<ToDoResponse>> handler,
                    CancellationToken cancellationToken) =>
                await handler.Handle(new GetAllToDosQuery(), cancellationToken))
            .IncludeInOpenApi()
            .WithTags(EndpointTag)
            .RequireRateLimiting("anonymous");
        app.MapGet("/{id:guid}", async (
                    IQueryHandler<GetToDoQuery, ToDoResponse?> handler,
                    Guid id,
                    CancellationToken cancellationToken) =>
                await handler.Handle(new GetToDoQuery(id), cancellationToken))
            .IncludeInOpenApi()
            .WithTags(EndpointTag)
            .RequireRateLimiting("anonymous");
        
        // POST
        app.MapPost("/", async (
                    ICommandHandler<CreateToDoCommand, ToDoResponse?> handler,
                    CreateToDoRequest createToDoDto,
                    CancellationToken cancellationToken) =>
                await handler.Handle(
                    new CreateToDoCommand(
                        createToDoDto.Title,
                        createToDoDto.Priority,
                        createToDoDto.Note,
                        createToDoDto.Reminder),
                    cancellationToken))
            .IncludeInOpenApi()
            .WithTags(EndpointTag)
            .RequireRateLimiting("anonymous");
        
        // DELETE
        app.MapDelete("/{id:guid}", async (
                    ICommandHandler<DeleteToDoCommand, bool> handler,
                    Guid id,
                    CancellationToken cancellationToken) =>
                await handler.Handle(new DeleteToDoCommand(id), cancellationToken))
            .IncludeInOpenApi()
            .WithTags(EndpointTag)
            .RequireRateLimiting("anonymous");
        
        // PATCH
        app.MapPatch("/", async (
                    ICommandHandler<UpdateToDoCommand, ToDoResponse?> handler,
                    UpdateToDoRequest updateToDoDto,
                    CancellationToken cancellationToken) =>
                await handler.Handle(
                    new UpdateToDoCommand(
                        updateToDoDto.Id,
                        updateToDoDto.Title,
                        updateToDoDto.Priority,
                        updateToDoDto.Note),
                    cancellationToken))
            .IncludeInOpenApi()
            .WithTags(EndpointTag)
            .RequireRateLimiting("anonymous");
    }
}
