using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Assignment1.Models;


public class UsersModel : PageModel
{
    private readonly UserManager<User> _userManager;

    public UsersModel(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public IList<User> Users { get; set; } = new List<User>();
    public IList<User> PagedUsers { get; set; } = new List<User>();

    // These properties are bound from the route (or query string if provided)
    public string Filter { get; set; } = "";
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int TotalPages { get; set; }

    public async Task OnGetAsync(int? pageNumber, string filter)
    {
        // If route parameters are not provided, use defaults.
        Filter = filter ?? "";
        CurrentPage = pageNumber ?? 1;

        IQueryable<User> query = _userManager.Users;

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            query = query.Where(u =>
                u.Email.Contains(Filter, StringComparison.OrdinalIgnoreCase) ||
                u.FirstName.Contains(Filter, StringComparison.OrdinalIgnoreCase) ||
                u.LastName.Contains(Filter, StringComparison.OrdinalIgnoreCase) ||
                u.Role.Contains(Filter, StringComparison.OrdinalIgnoreCase)
            );
        }

        Users = await query.ToListAsync();
        int count = Users.Count;
        TotalPages = (int)Math.Ceiling(count / (double)PageSize);

        // Ensure the current page is within valid bounds.
        if (CurrentPage < 1)
            CurrentPage = 1;
        if (CurrentPage > TotalPages)
            CurrentPage = TotalPages;

        PagedUsers = Users.Skip((CurrentPage - 1) * PageSize)
                          .Take(PageSize)
                          .ToList();
    }
}
