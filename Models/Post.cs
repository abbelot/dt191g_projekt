using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dt191g_projekt.Models;

public class Post
{
    // Properties
    public int Id { get; set; }

    [Required(ErrorMessage = "Rubrik måste fyllas i")]
    [MaxLength(256, ErrorMessage = "Ange max 256 tecken")]
    [Display(Name = "Rubrik")]
    public string? Title { get; set; }

    [Required(ErrorMessage = "Innehåll måste fyllas i")]
    [Display(Name = "Innehåll")]
    public string? Content { get; set; }

    [Display(Name = "Skapad av")]
    public string? CreatedBy { get; set; }

    [Display(Name = "Datum")]
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    [Display(Name = "Bild")]
    public string? ImageName { get; set; }

    [NotMapped]
    [Display(Name = "Bild")]
    public IFormFile? ImageFile { get; set; }

    [Required(ErrorMessage = "Välj kategori")]
    [Display(Name = "Kategori")]
    public int? CategoryId { get; set; }

    [Display(Name = "Kategori")]
    public Category? Category { get; set; }
}