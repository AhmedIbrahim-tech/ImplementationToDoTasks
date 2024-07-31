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



public class ReplaceValueModel
{
    public string? ColumnName { get; set; } = string.Empty;
    public string? OldValue { get; set; } = string.Empty;
    public string? NewValue { get; set; } = string.Empty;
    public bool SpecifyTableColumn { get; set; } = false;
    public string TableName { get; set; } = string.Empty;
    public string SelectedColumnName { get; set; } = string.Empty;

}


public class TableValues
{
    public string TableName { get; set; }
    public List<string> Values { get; set; }
}
