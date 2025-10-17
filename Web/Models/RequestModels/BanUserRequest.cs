namespace Web.Models.RequestModels
{
    public class BanUserRequest
    {
        public string User { get; set; } = null!;
        public string Duration { get; set; } = null!;
    }
}