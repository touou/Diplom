using System.ComponentModel.DataAnnotations;

namespace WebVer.Models;

public class AddUserViewModel
{
    [Required]
    [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$")]
    public string Email { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Password { get; set; }
}