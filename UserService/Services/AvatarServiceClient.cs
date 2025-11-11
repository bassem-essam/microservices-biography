// Services/IAvatarServiceClient.cs
using Newtonsoft.Json;
using Steeltoe.Discovery;
using System.Text;
using UserService.Models;

namespace UserService.Services
{
    public interface IAvatarServiceClient
    {
        Task<AvatarResponse> UploadAvatarAsync(IFormFile avatarFile);
        Task<AvatarResponse> GenerateAvatarAsync(string username);
        Task<AvatarResponse> DeleteAvatarAsync(string avatarPath);
    }
}

// Services/AvatarServiceClient.cs

namespace UserService.Services
{
    public class AvatarServiceClient : IAvatarServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IDiscoveryClient _discoveryClient;
        private readonly ILogger<AvatarServiceClient> _logger;
        private const string AvatarServiceName = "AVATARSERVICE";

        public AvatarServiceClient(
            HttpClient httpClient, 
            IDiscoveryClient discoveryClient,
            ILogger<AvatarServiceClient> logger)
        {
            _httpClient = httpClient;
            _discoveryClient = discoveryClient;
            _logger = logger;
        }

        private async Task<string> GetAvatarServiceUrlAsync()
        {
            try
            {
                var instances = _discoveryClient.GetInstances(AvatarServiceName);
                var instance = instances?.FirstOrDefault();
                
                if (instance == null)
                {
                    throw new InvalidOperationException($"No instances of {AvatarServiceName} found in service registry");
                }

                var baseUrl = $"{instance.Uri.Scheme}://{instance.Uri.Host}:{instance.Uri.Port}";
                _logger.LogInformation($"Using AvatarService instance: {baseUrl}");
                
                return baseUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to discover AvatarService instance");
                throw;
            }
        }

        public async Task<AvatarResponse> UploadAvatarAsync(IFormFile avatarFile)
        {
            try
            {
                if (avatarFile == null || avatarFile.Length == 0)
                {
                    return new AvatarResponse 
                    { 
                        Success = false, 
                        Message = "No file provided" 
                    };
                }

                var baseUrl = await GetAvatarServiceUrlAsync();
                var uploadUrl = $"{baseUrl}/upload";

                using var content = new MultipartFormDataContent();
                using var fileStream = avatarFile.OpenReadStream();
                using var streamContent = new StreamContent(fileStream);
                
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(avatarFile.ContentType);
                content.Add(streamContent, "avatar", avatarFile.FileName);

                _logger.LogInformation($"Uploading avatar to: {uploadUrl}");
                
                var response = await _httpClient.PostAsync(uploadUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var avatarResponse = JsonConvert.DeserializeObject<AvatarResponse>(responseContent);
                    _logger.LogInformation("Avatar uploaded successfully");
                    return avatarResponse ?? new AvatarResponse { Success = true, Message = "Upload successful" };
                }
                else
                {
                    _logger.LogWarning($"Avatar upload failed with status: {response.StatusCode}, Response: {responseContent}");
                    return new AvatarResponse 
                    { 
                        Success = false, 
                        Message = $"Upload failed: {response.StatusCode}" 
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading avatar");
                return new AvatarResponse 
                { 
                    Success = false, 
                    Message = $"Upload error: {ex.Message}" 
                };
            }
        }

        public async Task<AvatarResponse> GenerateAvatarAsync(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    return new AvatarResponse 
                    { 
                        Success = false, 
                        Message = "Username is required" 
                    };
                }

                var baseUrl = await GetAvatarServiceUrlAsync();
                var generateUrl = $"{baseUrl}/generate";

                var requestBody = new GenerateAvatarRequest { Username = username };
                var jsonContent = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _logger.LogInformation($"Generating avatar for user: {username}");
                
                var response = await _httpClient.PostAsync(generateUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var avatarResponse = JsonConvert.DeserializeObject<AvatarResponse>(responseContent);
                    _logger.LogInformation($"Avatar generated successfully for user: {username}");
                    return avatarResponse ?? new AvatarResponse { Success = true, Message = "Generation successful" };
                }
                else
                {

                    _logger.LogWarning($"Avatar generation failed with status: {response.StatusCode}, Response: {responseContent}");
                    throw new OperationFailed("Failed to generate avatar");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating avatar");
                throw new OperationFailed($"Avatar Generation error: {ex.Message}" );
            }
        }

        public async Task<AvatarResponse> DeleteAvatarAsync(string avatarPath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(avatarPath))
                {
                    return new AvatarResponse 
                    { 
                        Success = false, 
                        Message = "Avatar path is required" 
                    };
                }

                var baseUrl = await GetAvatarServiceUrlAsync();
                var deleteUrl = $"{baseUrl}/delete";

                var requestBody = new DeleteAvatarRequest { AvatarPath = avatarPath };
                var jsonContent = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _logger.LogInformation($"Deleting avatar: {avatarPath}");
                
                var response = await _httpClient.PostAsync(deleteUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var avatarResponse = JsonConvert.DeserializeObject<AvatarResponse>(responseContent);
                    _logger.LogInformation($"Avatar deleted successfully: {avatarPath}");
                    return avatarResponse ?? new AvatarResponse { Success = true, Message = "Deletion successful" };
                }
                else
                {
                    _logger.LogWarning($"Avatar deletion failed with status: {response.StatusCode}, Response: {responseContent}");
                    return new AvatarResponse 
                    { 
                        Success = false, 
                        Message = $"Deletion failed: {response.StatusCode}" 
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting avatar");
                return new AvatarResponse 
                { 
                    Success = false, 
                    Message = $"Deletion error: {ex.Message}" 
                };
            }
        }
    }
}