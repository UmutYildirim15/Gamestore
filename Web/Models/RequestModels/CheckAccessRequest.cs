namespace Web.Models.RequestModels
{
    public class CheckAccessRequest
    {
        public string? TargetPage { get; set; }
        public string? TargetId { get; set; }
    }
}