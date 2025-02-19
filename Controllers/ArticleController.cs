using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Assignment1.Data;
using Assignment1.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment1.Controllers
{
    [Authorize]
    public class ArticleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;


        public ArticleController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                ViewBag.Approved = user?.Approved ?? false;
            }
            var articles = await _context.Articles
                .Include(a => a.Contributor)
                .ToListAsync();
            return View(articles);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Article model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Get the logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(); 
            }

            var article = new Article
            {
                Title = model.Title,
                Body = model.Body,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                CreateDate = DateTime.UtcNow,
                ContributorUsername = user.UserName,
                Contributor = user
            };

            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            return RedirectToAction("Display", "Article", new { id = article.ArticleId });
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Display(int id)
        {
            var article = await _context.Articles
            .Include(a => a.Contributor)
            .FirstOrDefaultAsync(a => a.ArticleId == id);

            if (article == null)
                {
                    return NotFound();
                }

            return View(article);
        }

    }
}