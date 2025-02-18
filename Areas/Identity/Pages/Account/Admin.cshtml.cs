using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Assignment1.Models;

namespace Assignment1.Areas.Identity.Pages.Account
{
    public class AdminModel : PageModel
    {
        private readonly UserManager<User> _userManager;

        public AdminModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public IList<User> Users { get; set; }

        public async Task OnGetAsync()
        {
            Users = _userManager.Users.ToList();
        }
    }
}
