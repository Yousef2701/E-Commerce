namespace E_Commerce.Domain.IdentityModels
{
    public class AuthModel
    {
        public string Message { get; set; }

        public bool IsAuthenticated { get; set; }

        public string? Phone { get; set; }

        public string? Role { get; set; }

        public string? Token { get; set; }

        public DateTime? ExpiresOn { get; set; }
    }
}
