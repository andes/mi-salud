namespace LachybsServices.Configuration;

public class LachybsOptions
{
    public const string SectionName = "ApiLASCHyBS";

    public string? BaseUrl { get; set; }

    public bool EstaConfigurado => !string.IsNullOrWhiteSpace(BaseUrl);
}
