namespace ProjectManagement.Domain.ProjectContext.Entities.ProjectOwnershipping;

public sealed record ProjectOwnerId(Guid Id)
{
    private ProjectOwnerId() : this(Guid.Empty) { } // ef core

    public static ProjectOwnerId Create(Guid id)
    {
        if (id == Guid.Empty)
            throw new InvalidOperationException("Идентификатор владельца проекта некорректный.");
        return new ProjectOwnerId(id);
    }
}