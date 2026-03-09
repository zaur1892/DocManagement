using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocManagement.Models
{
    public class DocumentListView
{
    public int? Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? DocumentStatusId { get; set; }
    public int? DocumentTypeId { get; set; }
    public string? UserId { get; set; }
    public string? Note { get; set; }

    public string? DocumentTypeName { get; set; }
    public string? StatusName { get; set; }
    public string? StatusColor { get; set; }

    public string? ZoneName { get; set; }
    public string? EquipmentName { get; set; }

        public string? StatusCode { get; set; }
    }
    }