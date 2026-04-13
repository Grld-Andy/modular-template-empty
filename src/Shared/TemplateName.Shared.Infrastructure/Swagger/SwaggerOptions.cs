namespace TemplateName.Shared.Infrastructure.Swagger;

public sealed class SwaggerOptions
{
    public bool ReDocEnabled { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public string Version { get; set; }
    public string RoutePrefix { get; set; }
    public string AppFolder { get; set; }
}