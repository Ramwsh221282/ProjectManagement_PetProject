using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.ProjectContext.Entities.ProjectOwnershipping;

public sealed record ProjectOwnerId(Guid Id)
{
    private ProjectOwnerId()
        : this(Guid.Empty) { } // ef core

    public static Result<ProjectOwnerId> Create(Guid id) =>
        id == Guid.Empty
            ? Error.InvalidFormat("Идентификатор владельца проекта некорректный.")
            : new ProjectOwnerId(id);
}
