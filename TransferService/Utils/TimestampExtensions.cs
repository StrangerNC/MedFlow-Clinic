using Google.Protobuf.WellKnownTypes;

namespace TransferService.Utils;

public static class TimestampExtensions
{
    public static DateTime ToDateTime(this Timestamp timestamp)
    {
        return timestamp.ToDateTime().ToUniversalTime(); // или .ToLocalTime(), зависит от контекста
    }
}