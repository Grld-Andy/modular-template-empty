using IdentityModel.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using TemplateName.Shared.Infrastructure.Auth;
using System;
using System.Linq;
using System.Net.Http;


namespace TemplateName.Shared.Infrastructure.Swagger;

public static class Extensions
{
    public static IServiceCollection AddSwaggerDocs(this IServiceCollection services, SwaggerOptions swaggerOptions,
        JwtOptions jwtOptions)
    {
        services.AddSingleton(jwtOptions);
        services.AddSingleton(swaggerOptions);
        services.AddSwaggerGen(swagger =>
        {
            //var discoveryDocument = GetDiscoveryDocument(jwtOptions);
            swagger.OperationFilter<AuthorizeOperationFilter>();
            swagger.OperationFilter<CustomHeaderFilter>();
            swagger.DescribeAllParametersInCamelCase();
            swagger.EnableAnnotations();
            swagger.CustomSchemaIds(x => x.FullName);
            swagger.SwaggerDoc("v1", CreateOpenApiInfo(swaggerOptions));

            swagger.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,

                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{jwtOptions.Authority}/connect/authorize"),
                        TokenUrl = new Uri($"{jwtOptions.Authority}/connect/token"),
                        //AuthorizationUrl = new Uri(discoveryDocument.AuthorizeEndpoint),
                        //TokenUrl = new Uri(discoveryDocument.TokenEndpoint),
                        Scopes = jwtOptions.Scopes.ToDictionary(x => x, x => "")
                    }
                },
                Description = ""
            });
        });
        return services
            ;
    }

    public static IApplicationBuilder UseSwaggerDocs(this IApplicationBuilder app)
    {
        var jwtOptions = app.ApplicationServices.GetRequiredService<JwtOptions>();
        var swaggerOptions = app.ApplicationServices.GetRequiredService<SwaggerOptions>();

        var routePrefix = string.IsNullOrWhiteSpace(swaggerOptions.RoutePrefix)
            ? string.Empty
            : swaggerOptions.RoutePrefix;

        app.UseSwagger();
        return swaggerOptions.ReDocEnabled
            ? app.UseReDoc(reDoc =>
            {
                reDoc.RoutePrefix = routePrefix;
                reDoc.SpecUrl($"/swagger/{swaggerOptions.Name}/swagger.json");
                reDoc.DocumentTitle = swaggerOptions.Name;
            })
            :
#if DEBUG
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{swaggerOptions.Name}/swagger.json",
                    $"{swaggerOptions.Title} {swaggerOptions.Version}");
                c.OAuthClientId(jwtOptions.ClientId);
                c.OAuthClientSecret(jwtOptions.ClientSecret);
                c.OAuthAppName(swaggerOptions.Title);
                c.RoutePrefix = routePrefix;
                c.OAuthScopeSeparator(" ");
                c.OAuthUsePkce();
            });

#else
            app.UseSwaggerUI(c =>
            {
              c.SwaggerEndpoint($"{swaggerOptions.AppFolder}/swagger/{swaggerOptions.Name}/swagger.json", $"{swaggerOptions.Title} {swaggerOptions.Version}");
                   c.OAuthClientId(jwtOptions.ClientId);
                   c.OAuthClientSecret(jwtOptions.ClientSecret);
                   c.OAuthAppName(swaggerOptions.Title);
                   c.RoutePrefix = routePrefix;
                   c.OAuthScopeSeparator(" ");
                   c.OAuthUsePkce();
            });
#endif
    }

    private static DiscoveryDocumentResponse GetDiscoveryDocument(JwtOptions options)
    {
        var client = new HttpClient();
        return client
            .GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = options.Authority,
                Policy =
                {
                    RequireHttps = options.RequireHttpsMetadata
                }
            })
            .GetAwaiter()
            .GetResult();
    }

    private static OpenApiInfo CreateOpenApiInfo(SwaggerOptions options)
    {
        return new OpenApiInfo()
        {
            Title = options.Title,
            Version = options.Version
        };
    }
}