namespace RecetarServices.Configuration;

public class RecetarOptions
{
    public const string SectionName = "ApiRecetar";

    public string? BaseUrl { get; set; }
    public string? AccessToken { get; set; }

    public bool EstaConfigurado =>
        !string.IsNullOrWhiteSpace(BaseUrl) && !string.IsNullOrWhiteSpace(AccessToken);
}
