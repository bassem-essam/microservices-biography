using MediatR;
using UserService.Data;
using UserService.Services;

public class FindUserByUsernameHandler : IRequestHandler<FindUserByUsername, UserDTO>
{
    private readonly AppDbContext _context;
    private readonly IAnalyticsServiceClient _analyticsServiceClient;
    private readonly EventPublisher _eventPublisher;
    public FindUserByUsernameHandler(
        AppDbContext context,
        IAnalyticsServiceClient analyticsServiceClient,
        EventPublisher eventPublisher)
    {
        _context = context;
        _analyticsServiceClient = analyticsServiceClient;
        _eventPublisher = eventPublisher;
    }
    public async Task<UserDTO> Handle(FindUserByUsername request, CancellationToken cancellationToken)
    {
        var user = _context.User.FirstOrDefault(x => x.Username == request.Username);
        if (user == null) throw new UserNotFound(request.Username);

        var dto = UserDTO.FromUser(user);
        // var dto = new UserDTO { Username = request.Username, Biography = "This is only a test" }; 

        if (request.ShouldNotifyVisit)
            _eventPublisher.PublishUserVisited(request.Username);

        var visitCount = await _analyticsServiceClient.GetUserVisitCount(request.Username);

        dto.VisitCount = visitCount;

        return dto;
    }
}