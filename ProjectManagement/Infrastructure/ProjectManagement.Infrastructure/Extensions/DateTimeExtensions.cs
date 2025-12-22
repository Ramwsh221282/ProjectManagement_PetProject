namespace ProjectManagement.Infrastructure.Extensions;

public static class DateTimeExtensions
{
    public static DateTime? ToUtc(this DateTime? dateTime)
    {
        return dateTime?.ToUniversalTime();
    }

    public static DateTime ToUtc(this DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Utc) return dateTime;
        DateTime utcDateTime = dateTime.ToUniversalTime();
        return utcDateTime;
    }
}