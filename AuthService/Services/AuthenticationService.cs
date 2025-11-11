using AuthService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Services
{
    public interface IAuthenticationService
    {
        Task<AuthResponse> Register(RegisterRequest model);
        Task<AuthResponse> Login(LoginRequest model);
        Task<UserInfoResponse> GetUserInfo(string username);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private  readonly IUserServiceClient _userServiceClient;
        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            IOptions<JwtSettings> jwtSettings,
            IUserServiceClient userServiceClient)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _userServiceClient = userServiceClient;
        }

        public async Task<AuthResponse> Register(RegisterRequest model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
            {
                var errors = new Dictionary<string, List<string>> {
                    { "Username", new List<string> { "Username already exists" } }
                };
                throw new ValidationException("Registration failed", errors);
            }

            var userResponse = await _userServiceClient.CreateUser(model.Username);

            if (!userResponse.Success)
            {
                var errors = new Dictionary<string, List<string>> {
                    { "Username", new List<string> { "User creation failed with message: " + userResponse.Message } }
                };
                throw new ValidationException("Registration failed", errors);
            }

            ApplicationUser user = new ApplicationUser()
            {
                UserName = model.Username,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                // Convert the IdentityResult errors to our structured dictionary format
                var errors = result.Errors
                    .GroupBy(e => MapErrorToProperty(e.Code))
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToList()
                    );

                throw new ValidationException("Registration failed", errors);
            }

            return await GenerateJwtToken(user);
        }

        public async Task<AuthResponse> Login(LoginRequest model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                throw new LoginException("Invalid username or password");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid)
            {
                throw new LoginException("Invalid username or password");
            }

            return await GenerateJwtToken(user);
        }

        public async Task<UserInfoResponse> GetUserInfo(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                var errors = new Dictionary<string, List<string>> {
                    { "Username", new List<string> { "User not found" } }
                };
                throw new ValidationException("User not found", errors);
            }

            return new UserInfoResponse
            {
                Username = user.UserName
            };
        }

        private async Task<AuthResponse> GenerateJwtToken(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(_jwtSettings.ExpirationInMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new AuthResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expires,
                Username = user.UserName
            };
        }

        // Helper method to map Identity error codes to property names
        private string MapErrorToProperty(string errorCode)
        {
            // Map common Identity error codes to property names
            if (errorCode.Contains("Password", StringComparison.OrdinalIgnoreCase))
                return "Password";
            if (errorCode.Contains("UserName", StringComparison.OrdinalIgnoreCase))
                return "Username";
            // Default mapping for other errors
            return "General";
        }
    }
}

