namespace AndesServices.Configuration;

public class AndesOptions
{
    public const string SectionName = "ApiAndes";

    public string? BaseUrl { get; set; }
    public string? TokenRestablecerPassword { get; set; }

    public bool EstaConfigurado => !string.IsNullOrWhiteSpace(BaseUrl);
}
