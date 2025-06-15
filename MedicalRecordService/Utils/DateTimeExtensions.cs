namespace MedicalRecordService.Utils;

public static class DateTimeExtensions
{
    public static DateTime ToUtcKind(this DateTime dateTime)
    {
        return dateTime.Kind == DateTimeKind.Utc 
            ? dateTime 
            : dateTime.ToUniversalTime();
    }
}