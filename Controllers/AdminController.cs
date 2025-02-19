using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Assignment1.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

public class AdminController : Controller
{
    private readonly UserManager<User> _userManager;
    private const int PageSize = 20;

    public AdminController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    // Display user list with filtering and pagination
    public async Task<IActionResult> Admin(string filter = "", int page = 1)
    {
        IQueryable<User> query = _userManager.Users;

        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(u =>
                u.Email.Contains(filter) ||
                u.FirstName.Contains(filter) ||
                u.LastName.Contains(filter) ||
                u.Role.Contains(filter));
        }

        var totalUsers = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalUsers / (double)PageSize);

        page = Math.Clamp(page, 1, totalPages);

        var users = await query
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        ViewBag.Filter = filter;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;

        return View(users);
    }

    // POST: /Admin/Approve
    [HttpPost]
    public async Task<IActionResult> Approve(string id, string filter, int page)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        // Toggle the Approved property
        user.Approved = !user.Approved;
        await _userManager.UpdateAsync(user);

        return RedirectToAction("Admin", new { filter, page });
    }

    // POST: /Admin/ChangeRole
    [HttpPost]
    public async Task<IActionResult> ChangeRole(string id, string filter, int page)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        // Toggle the user's role (customize this logic as needed)
        user.Role = user.Role == "Contributor" ? "Admin" : "Contributor";
        await _userManager.UpdateAsync(user);

        return RedirectToAction("Admin", new { filter, page });
    }

    // POST: /Admin/Delete
    [HttpPost]
    public async Task<IActionResult> Delete(string id, string filter, int page)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        await _userManager.DeleteAsync(user);
        return RedirectToAction("Admin", new { filter, page });
    }
}
