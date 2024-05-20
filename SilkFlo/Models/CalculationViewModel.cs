using System.ComponentModel.DataAnnotations;

namespace SilkFlo.Models
{
    public class CalculationViewModel
    {
        [Required(ErrorMessage = "Expression is required.")]
        public string Expression { get; set; }
        public double? Memory { get; set; }
    }
}
