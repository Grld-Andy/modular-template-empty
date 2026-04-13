namespace TemplateName.Shared.Infrastructure.Auth;

public class JwtOptions
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string Audience { get; set; }
    public string Authority { get; set; }
    public string MetadataAddress { get; set; }
    public bool RequireHttpsMetadata { get; set; } = true;
    public string[] Scopes { get; set; }
}