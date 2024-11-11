using CurriculumVitaeManagementAPI.Interfaces;
using Microsoft.AspNetCore.Antiforgery;

namespace CurriculumVitaeManagementAPI.Services
{
    public class SessionValidationService(IAntiforgery antiforgery, IHttpContextAccessor httpContextAccessor) : ISessionValidationService
    {
        public object GetCsrfToken()
        {
            var httpContext = httpContextAccessor.HttpContext;
            var tokens = antiforgery.GetAndStoreTokens(httpContext);

            return new { csrfToken = tokens.RequestToken };
        }
    }
}
