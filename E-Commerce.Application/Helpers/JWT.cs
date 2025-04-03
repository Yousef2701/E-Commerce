namespace E_Commerce.Application.Helpers
{
    public class JWT
    {
        public string key { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string DurationInMinutes { get; set; }
    }
}
