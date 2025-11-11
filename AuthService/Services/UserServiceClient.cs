using System.Text;
using AuthService.Models;
using Newtonsoft.Json;
using Steeltoe.Discovery;

namespace AuthService.Services
{
    public interface IUserServiceClient
    {
        // Task<AvatarResponse> UploadAvatarAsync(IFormFile avatarFile);
        Task<UserServiceResponse> CreateUser(string username);
    }
}

// Services/UserServiceClient.cs

namespace AuthService.Services
{
    public class UserServiceClient : IUserServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IDiscoveryClient _discoveryClient;
        private readonly ILogger<UserServiceClient> _logger;
        private const string UserServiceName = "USERSERVICE";

        public UserServiceClient(
            HttpClient httpClient,
            IDiscoveryClient discoveryClient,
            ILogger<UserServiceClient> logger)
        {
            _httpClient = httpClient;
            _discoveryClient = discoveryClient;
            _logger = logger;
        }

        private async Task<string> GetUserServiceUrlAsync()
        {
            try
            {
                var instances = _discoveryClient.GetInstances(UserServiceName);
                var instance = instances?.FirstOrDefault();

                if (instance == null)
                {
                    throw new InvalidOperationException($"No instances of {UserServiceName} found in service registry");
                }

                var baseUrl = $"{instance.Uri.Scheme}://{instance.Uri.Host}:{instance.Uri.Port}";
                _logger.LogInformation($"Using UserService instance: {baseUrl}");

                return baseUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to discover UserService instance");
                throw;
            }
        }


        // public async Task<AvatarResponse> GenerateAvatarAsync(string username)
        // {
        //     try
        //     {
        //         if (string.IsNullOrWhiteSpace(username))
        //         {
        //             return new AvatarResponse 
        //             { 
        //                 Success = false, 
        //                 Message = "Username is required" 
        //             };
        //         }

        //         var baseUrl = await GetUserServiceUrlAsync();
        //         var generateUrl = $"{baseUrl}/generate";

        //         var requestBody = new GenerateAvatarRequest { Username = username };
        //         var jsonContent = JsonConvert.SerializeObject(requestBody);
        //         var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        //         _logger.LogInformation($"Generating avatar for user: {username}");

        //         var response = await _httpClient.PostAsync(generateUrl, content);
        //         var responseContent = await response.Content.ReadAsStringAsync();

        //         if (response.IsSuccessStatusCode)
        //         {
        //             var avatarResponse = JsonConvert.DeserializeObject<AvatarResponse>(responseContent);
        //             _logger.LogInformation($"Avatar generated successfully for user: {username}");
        //             return avatarResponse ?? new AvatarResponse { Success = true, Message = "Generation successful" };
        //         }
        //         else
        //         {
        //             _logger.LogWarning($"Avatar generation failed with status: {response.StatusCode}, Response: {responseContent}");
        //             return new AvatarResponse 
        //             { 
        //                 Success = false, 
        //                 Message = $"Generation failed: {response.StatusCode}" 
        //             };
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error generating avatar");
        //         return new AvatarResponse 
        //         { 
        //             Success = false, 
        //             Message = $"Generation error: {ex.Message}" 
        //         };
        //     }
        // }

        public async Task<UserServiceResponse> CreateUser(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    return new UserServiceResponse { Message = "Username is required" };
                }

                var baseUrl = await GetUserServiceUrlAsync();
                var createUserUrl = $"{baseUrl}/internal_api/users/create";

                var requestBody = new { Username = username };
                var jsonContent = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _logger.LogInformation($"Creating user: {username}");

                var response = await _httpClient.PostAsync(createUserUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"User creation failed with status: {response.StatusCode}");
                    return JsonConvert.DeserializeObject<UserServiceResponse>(await response.Content.ReadAsStringAsync());
                }

                _logger.LogInformation($"User created successfully: {username}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return new UserServiceResponse { Message = $"User Creation error: {ex.Message}" };
            }

            return new UserServiceResponse { Success = true, Message = "User created successfully" };
        }
    }
}
