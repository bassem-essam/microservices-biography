using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Services;

public class FindTopUsers : IRequest<List<UserDTO>>
{
    public int Limit { get; set; }
    // public int PageSize { get; set; }
    // public int PageNumber { get; set; }
}

public class FindTopUsersHandler : IRequestHandler<FindTopUsers, List<UserDTO>>
{
    private readonly AppDbContext _context;
    private readonly IAnalyticsServiceClient _analyticsServiceClient;
    public FindTopUsersHandler(AppDbContext context, IAnalyticsServiceClient analyticsServiceClient)
    {
        _context = context;
        _analyticsServiceClient = analyticsServiceClient;
    }

    public async Task<List<UserDTO>> Handle(FindTopUsers request, CancellationToken cancellationToken)
    {
        var topUsersResponse = await _analyticsServiceClient.GetTopVisitedUsernames(request.Limit);
        List<UserDTO> userDTOs = new List<UserDTO>();

        foreach (var topUser in topUsersResponse.Users)
        {
            var user = _context.User.FirstOrDefault(x => x.Username == topUser.UserId);
            if (user == null) throw new UserNotFound(topUser.UserId);

            UserDTO userDTO = UserDTO.FromUser(user);

            userDTO.VisitCount = topUser.VisitCount;
            userDTOs.Add(userDTO);
        }

        return userDTOs;
        // if (request.PageSize == 0) request.PageSize = 5;
        // if (request.PageNumber == 0) request.PageNumber = 1;

        // int offset = request.PageSize * request.PageNumber;
        // // List<UserDTO> users = await _context.User.Skip(offset).Take(request.PageSize).ToListAsync();
        // List<UserDTO> users = await _context.User.Take(request.PageSize).ToListAsync();
        // return users;
    }
}