using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using System.Web.Http.Results;

public class JwtAuthenticationFilter : IAuthenticationFilter
{
    private readonly TokenValidationParameters _params;
    public bool AllowMultiple => false;

    public JwtAuthenticationFilter(TokenValidationParameters parameters)
    {
        _params = parameters;
    }

    public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
    {
        var request = context.Request;
        var authorization = request.Headers.Authorization;

        if (authorization == null || authorization.Scheme != "Bearer" || string.IsNullOrEmpty(authorization.Parameter))
            return Task.CompletedTask;

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(authorization.Parameter, _params, out _);
            context.Principal = principal;
        }
        catch (Exception)
        {
            context.ErrorResult = new UnauthorizedResult(new System.Net.Http.Headers.AuthenticationHeaderValue[0], request);
        }

        return Task.CompletedTask;
    }

    public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        => Task.CompletedTask;
}