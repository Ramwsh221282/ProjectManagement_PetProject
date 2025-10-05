using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.UsersContext.ValueObjects;

namespace ProjectManagement.Domain.UsersContext.Database;

/// <summary>
/// Конфигурация модели Пользователя с базой данных
/// </summary>
public sealed class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // в таблицу "users@
        builder.ToTable("users");

        // ключ таблицы user_id
        builder.HasKey(x => x.UserId).HasName("user_id");

        // конфигурация свойства phoneNumber как столбца таблицы phone_number
        builder
            .Property(x => x.PhoneNumber)
            .IsRequired()
            .HasColumnName("phone_number")
            .HasConversion(toDb => toDb.Phone, fromDb => UserPhoneNumber.Create(fromDb));

        // конфигурация свойства registrationDate как столбца таблицы registration_date
        builder
            .Property(x => x.RegistrationDate)
            .IsRequired()
            .HasColumnName("registration_date")
            .HasConversion(toDb => toDb.Value, fromDb => UserRegistrationDate.Create(fromDb));

        // конфигурация статуса пользователя (сложного объекта)
        builder.ComplexProperty(
            x => x.Status,
            cpb =>
            {
                cpb.Property(s => s.Name).IsRequired().HasColumnName("status_name");
                cpb.Property(s => s.Value).IsRequired().HasColumnName("status_value");
            }
        );

        // конфигурация данных пользователя (сложного объекта)
        builder.ComplexProperty(
            x => x.AccountData,
            cpb =>
            {
                cpb.Property(x => x.Email).IsRequired().HasColumnName("email");
                cpb.Property(x => x.Login).IsRequired().HasColumnName("login");
            }
        );
    }
}
