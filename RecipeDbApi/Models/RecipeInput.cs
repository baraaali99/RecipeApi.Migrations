namespace RecipeDbApi.Models
{
    public class RecipeInput
    {
        public string Title { get; set; }
        public int Ingredients { get; set; } 
        public int Instructions { get; set; } 
        public List<int> Categories { get; set; } = new List<int>();
    }
}
