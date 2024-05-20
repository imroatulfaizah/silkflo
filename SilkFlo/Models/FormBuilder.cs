using System.ComponentModel.DataAnnotations;

namespace SilkFlo.Models
{
    public class FormBuilder
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
