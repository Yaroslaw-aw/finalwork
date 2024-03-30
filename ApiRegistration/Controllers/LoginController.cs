using ApiRegistration.AuthorizationModel;
using ApiRegistration.Db;
using ApiRegistration.Dto;
using ApiRegistration.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiRegistration.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : Controller
    {
        private readonly IUserAuthenticationService authenticationService;

        public LoginController(IUserAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        private static UserRole RoleIdToUserRole(RoleId roleId)
        {
            if (roleId is RoleId.Admin) return UserRole.Administrator;

            return UserRole.User;
        }

        [AllowAnonymous]
        [HttpPost(template: nameof(Login))]
        public async Task<ActionResult<string>> Login([FromBody] LoginModel userLogin)
        {
            try
            {
                string token = await authenticationService.AuthenticateAsync(userLogin);

                return Accepted(nameof(Login), token);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        
    }
}
