namespace RecipeDbApi.Models
{
    public class RecipeInput
    {
        public string Title { get; set; }
        public int IngredientsId { get; set; } 
        public int InstructionsId { get; set; } 
        public List<int> Categories { get; set; } = new List<int>();
        public int Id { get; set; }
    }
}
