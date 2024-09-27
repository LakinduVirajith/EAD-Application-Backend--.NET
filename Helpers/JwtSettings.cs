namespace EAD_Backend_Application__.NET.Helpers
{
    public class JwtSettings
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public double ExpiryInHours { get; set; }
        public double RefreshTokenExpiryInDays { get; set; }

        public JwtSettings()
        {
            
        }
    }
}
