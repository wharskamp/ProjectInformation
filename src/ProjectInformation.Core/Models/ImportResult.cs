namespace ProjectInformation.Core.Models;

public sealed record ImportResult(bool Success, string Message)
{
    public static ImportResult NotStarted { get; } = new(false, "No import has been started.");
}
