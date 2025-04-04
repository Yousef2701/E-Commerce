using E_Commerce.Domain.IdentityModels;
using E_Commerce.Domain.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        #region Dependancey injuction

        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        #endregion


        #region Register Async

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {
            if(model != null)
            {
                if (ModelState.IsValid)
                {
                    var result = await _authRepository.RegisterAsync(model);

                    if (!result.IsAuthenticated)
                        return BadRequest(result.Message);

                    return Ok(result);
                }
                return BadRequest(ModelState);
            }
           return BadRequest();
        }

        #endregion

        #region Login Async

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel model)
        {
            if(model != null)
            {
                if (ModelState.IsValid)
                {
                    var result = await _authRepository.LoginAsync(model);

                    if (!result.IsAuthenticated)
                        return BadRequest(result.Message);

                    return Ok(result);
                }
                return BadRequest(ModelState);
            }
            return BadRequest();
        }

        #endregion

    }
}
