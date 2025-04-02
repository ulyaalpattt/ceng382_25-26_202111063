using System.ComponentModel.DataAnnotations;

namespace Labwork5.Models
{
    public class ClassInformationModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Class name is required")]
    public string ClassName { get; set; } = string.Empty;

    [Range(1, 1000, ErrorMessage = "Student count must be between 1 and 1000")]
    public int StudentCount { get; set; }

    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; } = string.Empty;

    }
}
