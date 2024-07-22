using System.ComponentModel.DataAnnotations;

namespace ImplementationToDoTasks.Models;

public class ReplaceFileRenameModel
{
    [Required]
    [Display(Name = "Directory Path")]
    public string? DirectoryPath { get; set; }

    [Required]
    [Display(Name = "Search Pattern")]
    public string? SearchPattern { get; set; }

    [Display(Name = "Replace Pattern")]
    public string? ReplacePattern { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Contains Number")]
    public bool ContainsNumber { get; set; }
}