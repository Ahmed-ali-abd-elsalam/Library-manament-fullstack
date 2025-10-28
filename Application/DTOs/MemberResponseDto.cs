namespace Application.DTOs
{
    public record MemberResponseDto
    {
        public string Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int lateReturns { get; set; }
    }
}
