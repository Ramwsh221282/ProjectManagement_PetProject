using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectManagement.Infrastructure.Migrations;

public sealed class UserEntity_UniqueEmail_And_Login : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("CREATE UNIQUE INDEX IF NOT EXISTS idx_users_email_unique ON users(email);");
        migrationBuilder.Sql("CREATE UNIQUE INDEX IF NOT EXISTS idx_users_login_unique ON users(login);");
    }
}