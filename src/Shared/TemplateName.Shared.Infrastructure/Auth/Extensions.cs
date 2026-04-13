using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TemplateName.Shared.Abstractions.Modules;
using TemplateName.Shared.Abstractions.Auth;
using System.Threading.Tasks;

namespace TemplateName.Shared.Infrastructure.Auth;

public static class Extensions
{
    public static IServiceCollection AddAuth(this IServiceCollection services, IList<IModule> modules = null)
    {
        var options = services.GetOptions<JwtOptions>("jwt");


        services
            .AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.Authority = options.Authority;
                o.TokenValidationParameters.ValidateAudience = false;
                o.RequireHttpsMetadata = options.RequireHttpsMetadata;
                o.MetadataAddress = options.MetadataAddress;
                o.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
                o.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken))
                            // Read the token out of the query string
                            context.Token = accessToken;
                        return Task.CompletedTask;
                    }
                };
            });


        services.AddAuthorization(options =>
        {
            options.AddPolicy("TokenScopePolicy", builder =>
            {
                builder.RequireAuthenticatedUser()
                    .RequireClaim("scope", "lunch_api");
            });


            options.AddPolicy(HasPermissionAttribute.HasPermissionPolicyName, policyBuilder =>
            {
                policyBuilder.Requirements.Add(new HasPermissionAuthorizationRequirement());
            });


            //options.AddPolicy("SearchPolicy", builder =>
            //{
            //    builder.RequireAuthenticatedUser();
            //    builder.RequireAssertion(ctx =>
            //    {
            //        return true;
            //        //if (ctx.User.HasClaim("role", "Admin") ||
            //        //    ctx.User.HasClaim("role", "user"))
            //        //{
            //        //    return true;
            //        //}
            //        //return false;
            //    });
            //});
        });


        services.AddSingleton(options);
        services.AddScoped<IPasswordGenerator, PasswordGenerator>();

        return services;
    }

    public static IApplicationBuilder UseAuth(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers().RequireAuthorization("TokenScopePolicy");
        });


        return app;
    }
}