using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ConferenceBooking.Tests.Integration;

// Pause after PostgreSQL has acquired the room lock, not before sending SQL.
internal sealed class RowLockGate : DbCommandInterceptor
{
    private readonly bool _holdAfterLock;

    public RowLockGate(bool holdAfterLock)
    {
        _holdAfterLock = holdAfterLock;
    }

    public TaskCompletionSource Started { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public TaskCompletionSource Locked { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public TaskCompletionSource Release { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        if (command.CommandText.Contains("FOR UPDATE"))
        {
            Started.TrySetResult();
        }
        return ValueTask.FromResult(result);
    }

    public override async ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command, CommandExecutedEventData eventData, DbDataReader result,
        CancellationToken cancellationToken = default)
    {
        if (command.CommandText.Contains("FOR UPDATE"))
        {
            Locked.TrySetResult();
            if (_holdAfterLock)
            {
                await Release.Task.WaitAsync(TimeSpan.FromSeconds(15), cancellationToken);
            }
        }
        return result;
    }
}
