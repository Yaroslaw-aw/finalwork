using ApiRegistration.AuthorizationModel;
using ApiRegistration.Db;
using ApiRegistration.rsa;
using ApiRegistration.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiRegistration.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : Controller
    {
        private readonly IConfiguration config;
        private readonly IUserAuthenticationService authenticationService;
        private readonly IUserRepository userRepository;

        public LoginController(RsaTools rsaTools, IConfiguration config, IUserAuthenticationService authenticationService, IUserRepository userRepository)
        {
            this.config = config;
            this.authenticationService = authenticationService;
            this.userRepository = userRepository;
        }

        private static UserRole RoleIdToUserRole(RoleId roleId)
        {
            if (roleId is RoleId.Admin) return UserRole.Administrator;

            return UserRole.User;
        }

        [AllowAnonymous]
        [HttpPost(template: "Login")]
        public async Task<ActionResult> Login([FromBody] LoginModel userLogin)
        {
            try
            {
                User? user = await userRepository.CheckUserAsync(userLogin.Email, userLogin.Password);

                UserModel userModel = new UserModel
                {
                    UserEmail = userLogin.Email,
                    Password = userLogin.Password,
                    Role = RoleIdToUserRole(user.RoleId),
                    userId = user.userId
                };

                string token = GenerateToken(userModel);

                return Accepted(nameof(Login), token);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        private string GenerateToken(UserModel user)
        {
            RsaSecurityKey? key = new RsaSecurityKey(RsaTools.GetPrivateKey());
            SigningCredentials? credentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256Signature);

            Claim[]? claim = new[]
            {
                new Claim(ClaimTypes.PrimarySid, user.userId.ToString()),
                new Claim(ClaimTypes.Email, user.UserEmail),
                new Claim(ClaimTypes.Role, user.Role.ToString()),                
            };

            JwtSecurityToken? token = new JwtSecurityToken(config["Jwt:Issuer"],
                config["Jwt:Audience"],
                claim,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
