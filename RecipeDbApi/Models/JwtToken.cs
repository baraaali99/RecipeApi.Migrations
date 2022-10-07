namespace RecipeDbApi.Models
{
    public class JwtToken
    {
        public string Token { get; set; } = String.Empty;
        public string RefreshToken { get; set; } = String.Empty;
    }
}
