using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Data;

public class SearchUsers : IRequest<List<User>>
{
    public string SearchTerm { get; set; }
}

public class SearchUsersHandler : IRequestHandler<SearchUsers, List<User>>
{
    private readonly AppDbContext _context;
    public SearchUsersHandler(AppDbContext context)
    {
        _context = context;   
    }

    public async Task<List<User>> Handle(SearchUsers request, CancellationToken cancellationToken)
    {
        List<User> users = await _context.User.Where(x =>
            x.Name.ToLower().Contains(request.SearchTerm.ToLower()) || x.Username.ToLower().Contains(request.SearchTerm.ToLower())).ToListAsync();
        return users;
    }
}