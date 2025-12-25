using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.ProjectContext.Entities.ProjectOwnershipping;

public sealed record ProjectOwnerId(Guid Id)
{
    private ProjectOwnerId() : this(Guid.Empty) { } // ef core

    public static Result<ProjectOwnerId, Error> Create(Guid id)
    {
        if (id == Guid.Empty)
            return Failure<ProjectOwnerId, Error>(Error.InvalidFormat("Идентификатор владельца проекта некорректный."));
        return Success<ProjectOwnerId, Error>(new ProjectOwnerId(id));
    }
}