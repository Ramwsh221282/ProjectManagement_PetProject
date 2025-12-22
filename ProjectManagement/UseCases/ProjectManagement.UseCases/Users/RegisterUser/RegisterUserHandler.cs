using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.UserContext;
using ProjectManagement.Domain.UserContext.ValueObjects;

namespace ProjectManagement.UseCases.Users.RegisterUser;

public sealed class RegisterUserHandler
{
    private IUsersRepository Users { get; }
    private IUnitOfWork UnitOfWork { get; }

    public RegisterUserHandler(IUsersRepository users, IUnitOfWork unitOfWork)
    {
        Users = users;
        UnitOfWork = unitOfWork;
    }
    
    
    public async Task<User> Handle(RegisterUserCommand command, CancellationToken ct = default)
    {
        User user = await User.CreateNew(
            command.Email, 
            command.Login, 
            command.Phone, 
            Users, ct);
        
        await UnitOfWork.SaveChangesAsync(ct);
        
        return user;
    }
}