using System;
using Microsoft.AspNetCore.Builder;

namespace employeeservice.Common
{
    /// <summary>
    /// Referred : https://andrewlock.net/adding-default-security-headers-in-asp-net-core/
    /// </summary>
    public static class MyMiddlewareExtensions
    {

        /// <summary>
        /// This method is used to for security headers middleware .
        /// <param name="app"></param>
        /// <param name="builder"></param>
        /// </summary>
        /// <returns></returns>
        public static IApplicationBuilder UseSecurityHeadersMiddleware(this IApplicationBuilder app, SecurityHeadersBuilder builder)
        {
            SecurityHeadersPolicy policy = builder.Build();
            return app.UseMiddleware<SecurityHeadersMiddleware>(policy);
        }
    }
}
