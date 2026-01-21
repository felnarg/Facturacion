using Compras.Application.Interfaces;

namespace Compras.Infrastructure.Time;

public sealed class SystemClock : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
