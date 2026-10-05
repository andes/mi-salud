namespace AdminLogsServices.Configuration;

public sealed class AdminIngestionOptions
{
    public const string SectionName = "AdminLogs";

    public string? BaseUrl { get; set; }
    public string? ApiKey { get; set; }
    public int QueueCapacity { get; set; } = 1000;
    public int MaxAttempts { get; set; } = 5;
    public int InitialRetryDelayMs { get; set; } = 500;
    public int MaxRetryDelayMs { get; set; } = 30_000;
    public int HttpTimeoutSeconds { get; set; } = 10;
    public int DrainTimeoutSeconds { get; set; } = 5;

    public bool EstaConfigurado => !string.IsNullOrEmpty(ApiKey);
}
