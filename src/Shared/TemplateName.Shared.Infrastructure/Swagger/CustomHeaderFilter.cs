using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace TemplateName.Shared.Infrastructure.Swagger
{
    internal class CustomHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {

            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = "CompanyReference",
                In = ParameterLocation.Header,
                Required = false
            });

            //operation.Parameters.Add(new OpenApiParameter()
            //{
            //    Name = "ApplicationId",
            //    In = ParameterLocation.Header,
            //    Required = true
            //});
        }
    }
}
