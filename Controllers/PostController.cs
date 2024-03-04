using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using dt191g_projekt.Data;
using dt191g_projekt.Models;
using Microsoft.AspNetCore.Authorization;

namespace dt191g_projekt.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly string wwwRootPath;

        public PostController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            wwwRootPath = hostEnvironment.WebRootPath; //path to images folder in wwwroot
        }

        // GET: Post
        [Route("/nyheter")]
        public async Task<IActionResult> Index()
        {
            if (_context.Posts == null)
            {
                return NotFound();
            }

            var applicationDbContext = _context.Posts.Include(p => p.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Post/Details/5
        [Route("/nyheter/detaljer/{id}")]
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

        // GET: Post/Create
        [Route("/nyheter/skapa-nyhet")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Post/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("/nyheter/skapa-nyhet")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,ImageFile,CategoryId")] Post post)
        {
            if (ModelState.IsValid)
            {
                // Check if image exist in post
                if (post.ImageFile != null)
                {
                    // Generate unique file name and save as ImageName
                    string fileName = Path.GetFileNameWithoutExtension(post.ImageFile.FileName);
                    string extension = Path.GetExtension(post.ImageFile.FileName);

                    post.ImageName = fileName = fileName.Replace(" ", String.Empty) + DateTime.Now.ToString("yymmssfff") + extension;

                    string path = Path.Combine(wwwRootPath + "/images", fileName);

                    // Save in folder path
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await post.ImageFile.CopyToAsync(fileStream);
                    }
                }

                _context.Add(post);

                // Add logged in user to CreatedBy
                post.CreatedBy = User.Identity?.Name ?? "Unknown";

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            return View(post);

        }

        // GET: Post/Edit/5
        [Route("/nyheter/redigera/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (_context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            return View(post);
        }

        // POST: Post/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/nyheter/redigera/{id}")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,ImageFile,CategoryId")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Check if image exist in post
                    if (post.ImageFile != null)
                    {
                        // Generate unique file name and save as ImageName
                        string fileName = Path.GetFileNameWithoutExtension(post.ImageFile.FileName);
                        string extension = Path.GetExtension(post.ImageFile.FileName);

                        post.ImageName = fileName = fileName.Replace(" ", String.Empty) + DateTime.Now.ToString("yymmssfff") + extension;

                        string path = Path.Combine(wwwRootPath + "/images", fileName);

                        // Save in folder path
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await post.ImageFile.CopyToAsync(fileStream);
                        }
                    }
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            return View(post);
        }

        // GET: Post/Delete/5
        [Route("/nyheter/radera/{id}")]
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("/nyheter/radera/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            if (_context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                if (!string.IsNullOrEmpty(post.ImageName))
                {

                    // Construct path to image file
                    string imagePath = Path.Combine(wwwRootPath + "/images", post.ImageName);

                    // Check if image exists
                    if (System.IO.File.Exists(imagePath))
                    {
                        // Delete image
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {

            if (_context.Posts == null)
            {
                return false;
            }

            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
