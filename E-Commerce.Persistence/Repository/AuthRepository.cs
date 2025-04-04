using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using E_Commerce.Application.Enums;
using E_Commerce.Application.Helpers;
using E_Commerce.Domain.IdentityModels;
using E_Commerce.Domain.IRepository;
using E_Commerce.Persistence.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace E_Commerce.Persistence.Repository
{
    public class AuthRepository : IAuthRepository
    {

        #region Dependancey injuction

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JWT _jWT;

        public AuthRepository(UserManager<ApplicationUser> userManager, IOptions<JWT> jwt)
        {
            _userManager = userManager;
            _jWT = jwt.Value;
        }

        #endregion


        #region Register Async

        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            if (model != null)
            {
                if (await _userManager.FindByEmailAsync(model.Email) is not null)
                    return new AuthModel { Message = "Email is already registered!" };

                if (await _userManager.FindByNameAsync(model.Username) is not null)
                    return new AuthModel { Message = "Username is already registered!" };

                var user = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Empty;

                    foreach (var error in result.Errors)
                    {
                        errors += $"{error.Description}, ";
                    }

                    return new AuthModel { Message = errors };
                }

                string role;
                switch (model.Role)
                {
                    case UsersRoles.Customer:
                        await _userManager.AddToRoleAsync(user, UsersRoles.Customer.ToString());
                        role = UsersRoles.Customer.ToString();
                        break;
                    case UsersRoles.Trader:
                        await _userManager.AddToRoleAsync(user, UsersRoles.Trader.ToString());
                        role = UsersRoles.Trader.ToString();
                        break;
                    case UsersRoles.Admin:
                        await _userManager.AddToRoleAsync(user, UsersRoles.Admin.ToString());
                        role = UsersRoles.Admin.ToString();
                        break;
                    case UsersRoles.ShippingMan:
                        await _userManager.AddToRoleAsync(user, UsersRoles.ShippingMan.ToString());
                        role = UsersRoles.ShippingMan.ToString();
                        break;
                    default:
                        await _userManager.AddToRoleAsync(user, UsersRoles.Customer.ToString());
                        role = UsersRoles.Customer.ToString();
                        break;
                }

                var jwtSecurityToken = await CreateJWTToken(user);

                return new AuthModel
                {
                    ExpiresOn = jwtSecurityToken.ValidTo,
                    Role = role,
                    IsAuthenticated = true,
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                    Phone = user.PhoneNumber
                };
            }
            throw new ArgumentNullException("model");
        }
        #endregion

        #region Login Async

        public async Task<AuthModel> LoginAsync(LoginModel model)
        {
            if (model != null) 
            {
                var authModel = new AuthModel();

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    authModel.Message = "Email or Password is incorrect!";
                    return authModel;
                }

                var jwtSecurityToken = await CreateJWTToken(user);

                var roles = await _userManager.GetRolesAsync(user);

                authModel.IsAuthenticated = true;
                authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                authModel.ExpiresOn = jwtSecurityToken.ValidTo;
                authModel.Phone = user.PhoneNumber;
                authModel.Role = roles.FirstOrDefault();

                return authModel;
            }
            throw new ArgumentNullException("model");
        }

        #endregion

        #region Create JWT Token

        private async Task<JwtSecurityToken> CreateJWTToken(ApplicationUser user)
        {
            if (user != null)
            {
                var userclaims = await _userManager.GetClaimsAsync(user);
                var roles = await _userManager.GetRolesAsync(user);
                var roleclaims = new List<Claim>();

                foreach (var role in roles)
                {
                    roleclaims.Add(new Claim("roles", role));
                }

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim("uid", user.Id)
                }
                .Union(userclaims)
                .Union(roleclaims);

                var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jWT.key));
                var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

                var jwtSecurityToken = new JwtSecurityToken(
                    issuer: _jWT.Issuer,
                    audience: _jWT.Audience,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(_jWT.DurationInMinutes),
                    signingCredentials: signingCredentials
                );

                return jwtSecurityToken;
            }
            throw new ArgumentNullException("user");
        }

        #endregion

    }
}
