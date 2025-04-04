using E_Commerce.Domain.IdentityModels;

namespace E_Commerce.Domain.IRepository
{
    public interface IAuthRepository
    {
        Task<AuthModel> RegisterAsync(RegisterModel model);

        Task<AuthModel> LoginAsync(LoginModel model);
    }
}
