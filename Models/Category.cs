using System.ComponentModel.DataAnnotations;

namespace dt191g_projekt.Models;

public class Category
{
    // Properties
    public int Id { get; set; }

    [Required(ErrorMessage = "Fyll i namn på kategorin")]
    [Display(Name = "Kategori")]
    public string? Name { get; set; }
    public List<Post>? Posts { get; set; }

}