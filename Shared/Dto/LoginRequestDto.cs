namespace Shared.Dto
{
    public class LoginRequestDto
    {
        public string? Login { get; set; }
        public string? Password { get; set; }
        public bool InternalAuth { get; set; }
    }
}