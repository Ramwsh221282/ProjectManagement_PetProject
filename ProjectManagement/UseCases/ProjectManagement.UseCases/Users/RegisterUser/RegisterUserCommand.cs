namespace ProjectManagement.UseCases.Users.RegisterUser;

public record RegisterUserCommand(
    string Email,
    string Login,
    string Phone
    );