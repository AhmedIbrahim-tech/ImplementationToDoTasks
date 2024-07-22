using System.ComponentModel.DataAnnotations;

namespace ImplementationToDoTasks.Models;

public class EncryptionModel
{
    [Required]
    [Display(Name = "Input Text")]
    public string? InputText { get; set; }

    public string? Result { get; set; }

    [Required]
    public string? Operation { get; set; }
}
