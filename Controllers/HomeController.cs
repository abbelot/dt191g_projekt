using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using dt191g_projekt.Models;
using dt191g_projekt.Data;
using Microsoft.EntityFrameworkCore;
using dt191g_projekt.Helpers;

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
        if (_context.Posts == null)
        {
            return NotFound();
        }
        // Search query
        var posts = _context.Posts
                    .Include(p => p.Category)
                    .OrderBy(p => p.Title) as IQueryable<Post>;

        if (!string.IsNullOrEmpty(searchString))
        {
            searchString = searchString.ToLower();
            // Case insensitive search, either for post title or post category
            posts = posts.Where(s => (s.Title != null && s.Title.ToLower().Contains(searchString)) || (s.Category != null && s.Category.Name != null && s.Category.Name.ToLower().Contains(searchString)));
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
        var posts = await _context.Posts.Include(p => p.Category).OrderByDescending(p => p.CreatedDate).Take(5).ToListAsync();
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

    [Route("/alla-nyheter")]
    public async Task<IActionResult> AllNews(int pageNumber = 1, int pageSize = 10)
    {
        if (_context.Posts == null)
        {
            return NotFound();
        }
        var postsQuery = _context.Posts.Include(p => p.Category).OrderByDescending(p => p.CreatedDate).AsNoTracking();
        var paginatedPosts = await PaginatedList<Post>.CreateAsync(postsQuery, pageNumber, pageSize);
        return View(paginatedPosts);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [Route("/om-sidan")]
    public IActionResult About()
    {
        return View();
    }
}
