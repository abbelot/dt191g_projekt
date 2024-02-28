using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using dt191g_projekt.Models;
using dt191g_projekt.Data;
using Microsoft.EntityFrameworkCore;

namespace dt191g_projekt.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    // Search news
    [Route("/sok-nyhet")]
    public IActionResult Search()
    {
        return View();
    }

    [Route("/sok-resultat")]
    public async Task<IActionResult> SearchResults(string searchString)
    {
        // Search query
        var posts = _context.Posts
                    .Include(p => p.Category)
                    .OrderBy(p => p.Title) as IQueryable<Post>;
        
        if (!string.IsNullOrEmpty(searchString))
        {
            // Case insensitive search, either for post title or post category
            posts = posts.Where(s => s.Title.ToLower().Contains(searchString.ToLower()) || s.Category.Name.ToLower().Contains(searchString.ToLower()));
        }

        ViewData["SearchString"] = searchString;
        return View(await posts.ToListAsync());
    }

    public async Task<IActionResult> Index()
    {
        if (_context.Posts == null)
        {
            return NotFound();
        }
        var posts = await _context.Posts.Include(p => p.Category).ToListAsync();
        return View(posts);
    }

    [Route("/nyhet/{id}")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        if (_context.Posts == null)
        {
            return NotFound();
        }

        var post = await _context.Posts
            .Include(p => p.Category)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (post == null)
        {
            return NotFound();
        }

        return View(post);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
