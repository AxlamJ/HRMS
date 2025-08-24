using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Logging;

namespace HrManagement.Filters
{
    public class AuthorizeFilter : ActionFilterAttribute
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AuthorizeFilter> _logger;

        public AuthorizeFilter(
            IHttpClientFactory httpClientFactory, 
            IConfiguration configuration,
            ILogger<AuthorizeFilter> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                //var token = context.HttpContext.Request.Cookies["jwtToken"];
                var token = context.HttpContext.Session.GetString("jwtToken");

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Unauthorized access attempt - No token found");
                    context.Result = new RedirectToActionResult("Login", "Home", null);
                    return;
                }

                // Validate token
                if (!ValidateToken(token))
                {
                    _logger.LogWarning("Unauthorized access attempt - Invalid token");
                    context.HttpContext.Session.Clear();
                    context.Result = new RedirectToActionResult("Login", "Home", null);
                    return;
                }

                // Add token to outgoing requests
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                
                // Add token to response headers
                context.HttpContext.Response.Headers.Add("Authorization", $"Bearer {token}");

                base.OnActionExecuting(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AuthorizeFilter");
                context.Result = new RedirectToActionResult("Login", "Home", null);
            }
        }

        private bool ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);
                
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token validation failed");
                return false;
            }
        }
    }
}
