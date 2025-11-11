using Grpc.Core;
using AnalyticsService.Services;

namespace AnalyticsService.Services;

public class AnalyticsGrpcService : Analytics.AnalyticsBase
{
    private readonly IAnalyticsRepository _repository;
    private readonly ILogger<AnalyticsGrpcService> _logger;

    public AnalyticsGrpcService(IAnalyticsRepository repository, ILogger<AnalyticsGrpcService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public override async Task<UserVisitResponse> GetUserVisitCount(UserVisitRequest request, ServerCallContext context)
    {
        try
        {
            var userAnalytics = await _repository.GetUserAnalyticsAsync(request.UserId);
            
            if (userAnalytics == null)
            {
                return new UserVisitResponse
                {
                    UserId = request.UserId,
                    VisitCount = 0,
                    FirstVisit = "",
                    LastVisit = ""
                };
            }

            return new UserVisitResponse
            {
                UserId = userAnalytics.UserId,
                VisitCount = userAnalytics.VisitCount,
                FirstVisit = userAnalytics.FirstVisit?.ToString("yyyy-MM-dd HH:mm:ss") ?? "",
                LastVisit = userAnalytics.LastVisit?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting visit count for user: {request.UserId}");
            throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
        }
    }

    public override async Task<UserAnalyticsResponse> GetUserAnalytics(UserAnalyticsRequest request, ServerCallContext context)
    {
        try
        {
            var userAnalytics = await _repository.GetUserAnalyticsAsync(request.UserId);
            
            if (userAnalytics == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }

            var response = new UserAnalyticsResponse
            {
                UserId = userAnalytics.UserId,
                VisitCount = userAnalytics.VisitCount,
                CreatedAt = userAnalytics.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                FirstVisit = userAnalytics.FirstVisit?.ToString("yyyy-MM-dd HH:mm:ss") ?? "",
                LastVisit = userAnalytics.LastVisit?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""
            };

            response.VisitDates.AddRange(userAnalytics.VisitDates.Select(d => d.ToString("yyyy-MM-dd HH:mm:ss")));

            return response;
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting analytics for user: {request.UserId}");
            throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
        }
    }

    public override async Task<TopUsersResponse> GetTopVisitedUsers(TopUsersRequest request, ServerCallContext context)
    {
        try
        {
            var topUsers = await _repository.GetTopUsersAsync(request.Limit);
            var response = new TopUsersResponse();

            response.Users.AddRange(topUsers.Select(user => new UserVisitResponse
            {
                UserId = user.UserId,
                VisitCount = user.VisitCount,
                FirstVisit = user.FirstVisit?.ToString("yyyy-MM-dd HH:mm:ss") ?? "",
                LastVisit = user.LastVisit?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""
            }));

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting top visited users");
            throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
        }
    }
}

