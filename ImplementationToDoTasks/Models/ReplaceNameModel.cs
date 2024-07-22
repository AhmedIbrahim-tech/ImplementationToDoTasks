using System.ComponentModel.DataAnnotations;

namespace ImplementationToDoTasks.Models;

public class ReplaceNameModel
{
    [Required]
    [Display(Name = "Old Name")]
    public string OldName { get; set; }

    [Required]
    [Display(Name = "New Name")]
    public string NewName { get; set; }
}
