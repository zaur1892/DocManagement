using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocManagement.Models
{ 

public class DocElement
{
    public int Id { get; set; }
    public string ColumnName { get; set; }
    public string DisplayName { get; set; }
    public bool IsVisible { get; set; }
    public int OrderIndex { get; set; }
}

}