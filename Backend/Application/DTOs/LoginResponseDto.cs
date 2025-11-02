namespace Application.DTOs
{
    public record LoginResponseDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Access_Token { get; set; } = string.Empty;
        public string Refresh_token { get; set; } = string.Empty;
    }
}
