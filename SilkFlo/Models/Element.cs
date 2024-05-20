namespace SilkFlo.Models
{
    public class Element
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string LabelText { get; set; }
        public string PlaceholderText { get; set; }
        public string OptionsText { get; set; }
        public int? IdFormBuilder { get; set; }
    }
}
