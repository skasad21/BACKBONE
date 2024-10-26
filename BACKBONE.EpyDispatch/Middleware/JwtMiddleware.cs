using BACKBONE.Application.Interfaces.SecurityInterface;
using BACKBONE.Core.Models;
using BACKBONE.Core.ResponseClasses;
using System.Collections.Generic;
using System.Text.Json;

namespace BACKBONE.EpyDispatch.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HashSet<string> _allowedPaths;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;

            // Initialize the list of allowed paths
            _allowedPaths = new HashSet<string>
            {
                "/api/Auth/login",
                "/api/Auth/refresh-token",
                "/api/User/withoutauth"
                // Add more paths here as needed
            };
        }

        public async Task Invoke(HttpContext context)
        {
            // Skip middleware for allowed endpoints
            if (IsPathAllowed(context.Request.Path.Value))
            {
                await _next(context);
                return;
            }

            var jwtTokenService = context.RequestServices.GetRequiredService<IJwtTokenService>();
            var refreshTokenService = context.RequestServices.GetRequiredService<IRefreshTokenService>();

            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                // No token provided in the request
                await RespondWithJson(context, "No token provided. Unauthorized access.", false);
                return;
            }

            var isValidToken = jwtTokenService.ValidateToken(token);

            if (isValidToken == null) // Invalid or expired JWT token
            {
                var refreshToken = context.Request.Headers["RefreshToken"].FirstOrDefault();

                if (!string.IsNullOrEmpty(refreshToken))
                {
                    var isValidRefreshToken = refreshTokenService.ValidateRefreshToken(refreshToken);

                    if (isValidRefreshToken)
                    {
                        var result = await refreshTokenService.GetRefreshTokenAsync(refreshToken);
                        if (result != null)
                        {
                            var newJwtToken = jwtTokenService.GenerateToken(new User { UserId = result.UserId });

                            context.Response.Headers.Add("NewJwtToken", newJwtToken);

                            // Structured response for new token issued
                            var response = new EQResponse<string>
                            {
                                Message = "New JWT token issued successfully.",
                                Success = true,
                                Data = new EQResponseData<string> { SingleValue = newJwtToken }
                            };

                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                            return;
                        }
                        else
                        {
                            await RespondWithJson(context, "Failed to retrieve user from refresh token.", false);
                            return;
                        }
                    }
                    else
                    {
                        await RespondWithJson(context, "Invalid refresh token. Please login again.", false);
                        return;
                    }
                }
                else
                {
                    await RespondWithJson(context, "JWT token expired. Please login again.", false);
                    return;
                }
            }

            // Continue to the next middleware if token is valid
            await _next(context);
        }

        private bool IsPathAllowed(string path)
        {
            // Check if the current request path is in the allowed paths
            //return _allowedPaths.Contains(path);
            return _allowedPaths.Any(allowedPath => path.StartsWith(allowedPath));
        }

        private async Task RespondWithJson(HttpContext context, string message, bool success)
        {
            var response = new EQResponse<string>
            {
                Message = message,
                Success = success,
                Data = null
            };

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
