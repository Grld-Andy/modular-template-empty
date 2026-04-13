using Microsoft.AspNetCore.Authorization;
using System;

namespace TemplateName.Shared.Infrastructure.Auth
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        internal const string HasPermissionPolicyName = "HasPermission";

        public HasPermissionAttribute(string name)
            : base(HasPermissionPolicyName)
        {
            Name = name;
        }

        public string Name { get; }
    }
}