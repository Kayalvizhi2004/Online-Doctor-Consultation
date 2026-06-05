namespace ConsultationApi.Configurations;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string SecretKey { get; set; } = default!;
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public int ExpiryMinutes { get; set; }
    public int RefreshTokenDays { get; set; }
}