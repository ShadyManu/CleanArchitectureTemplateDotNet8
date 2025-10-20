using Application.Dtos.ToDo.Response;
using Domain.Entities;
using Mapster;

namespace Application.Dtos;

public static class MapsterConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<ToDoEntity, ToDoResponse>.NewConfig();
    }
}
