using Grpc.Net.Client;
using Steeltoe.Discovery;
using UserService.Models;

namespace UserService.Services
{
    public interface IAnalyticsServiceClient
    {
        Task<int> GetUserVisitCount(string username);
        Task<TopUsersResponse> GetTopVisitedUsernames(int limit);
    }
}

// Services/AvatarServiceClient.cs

namespace UserService.Services
{
    public class AnalyticsServiceClient : IAnalyticsServiceClient
    {
        private readonly IDiscoveryClient _discoveryClient;
        private readonly ILogger<AvatarServiceClient> _logger;
        private Analytics.AnalyticsClient _client;
        private const string AnalyticsServiceName = "ANALYTICSSERVICE";
        public AnalyticsServiceClient(ILogger<AvatarServiceClient> logger,
            IDiscoveryClient discoveryClient)
        {
            _discoveryClient = discoveryClient;
            _logger = logger;

            CreateClient();
        }

        private string GetAnalyticsServiceUrlAsync()
        {
            try
            {
                var instances = _discoveryClient.GetInstances(AnalyticsServiceName);
                var instance = instances?.FirstOrDefault();

                if (instance == null)
                {
                    throw new InvalidOperationException($"No instances of {AnalyticsServiceName} found in service registry");
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

        private async void CreateClient()
        {
            var channel = GrpcChannel.ForAddress(GetAnalyticsServiceUrlAsync());
            _client = new Analytics.AnalyticsClient(channel);
        }

        public async Task<int> GetUserVisitCount(string username)
        {
            try
            {
                var response = await _client.GetUserVisitCountAsync(new UserVisitRequest { UserId = username });
                return response.VisitCount;
            }
            catch (Exception ex)
            { 
                throw new OperationFailed("Failed to get user visit count with error:" + ex.Message, ex);
            }
        }

        public async Task<TopUsersResponse> GetTopVisitedUsernames(int limit)
        {
            try
            {
                var response = await _client.GetTopVisitedUsersAsync(new TopUsersRequest { Limit = limit });
                return response;
            }
            catch (Exception ex)
            {
                throw new OperationFailed("Failed to get top visited usernames with error:" + ex.Message, ex);
            }
        }
    }
}