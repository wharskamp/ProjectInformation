namespace ProjectInformation.Core.Models;

public sealed record ImportResult(bool Success, string Message, int MailCount, int ContactCount)
{
    public static ImportResult NotStarted { get; } = new(false, "No import has been started.", 0, 0);
}
