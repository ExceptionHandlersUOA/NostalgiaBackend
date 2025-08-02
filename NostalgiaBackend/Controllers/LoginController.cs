using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace NostalgiaBackend.Controllers
{
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    [ApiController]
    [Route("api/login")]
    public class LoginController : Controller
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest();
            }

            var token = await ValidateCredentials(request.Username, request.Password);
            if (!string.IsNullOrEmpty(token))
            {
                return Ok(new
                {
                    token
                });
            }

            return Unauthorized();
        }

        private static async Task<string> ValidateCredentials(string username, string password)
        {
            var authentikServer = Environment.GetEnvironmentVariable("AUTHENTIK_SERVER");
            var clientId = Environment.GetEnvironmentVariable("AUTHENTIK_CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("AUTHENTIK_CLIENT_SECRET");

            if (string.IsNullOrEmpty(authentikServer))
            {
                return "placeholder-token";
            }

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                throw new InvalidOperationException("Authentik client credentials not configured");
            }

            using var httpClient = new HttpClient();

            var formData = new List<KeyValuePair<string, string>>
            {
                new("grant_type", "password"),
                new("client_id", clientId),
                new("client_secret", clientSecret),
                new("username", username),
                new("password", password),
                new("scope", "openid profile email")
            };

            var content = new FormUrlEncodedContent(formData);

            try
            {
                var response = await httpClient.PostAsync($"{authentikServer}/application/o/token/", content);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
                    
                    if (tokenResponse.TryGetProperty("access_token", out var accessToken))
                    {
                        return accessToken.GetString();
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
