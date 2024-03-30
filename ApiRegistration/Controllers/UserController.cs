using ApiRegistration.AuthorizationModel;
using ApiRegistration.Db;
using ApiRegistration.Dto;
using ApiRegistration.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace ApiRegistration.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly Redis redis;

        public UserController(IUserRepository userRepository, Redis redis)
        {
            this.userRepository = userRepository;
            this.redis = redis;
        }

        [AllowAnonymous]
        [HttpPost(template: nameof(AddUser))]
        public async Task<ActionResult<Guid?>> AddUser([FromBody] LoginModel loginModel)
        {
            Guid? userId = null;

            // Регулярное выражение для проверки адреса электронной почты
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            // Проверка с использованием регулярного выражения
            Regex regex = new Regex(pattern);

            if (loginModel.Email is not null)
            {
                bool isValidEmail = regex.IsMatch(loginModel.Email);
                if (!isValidEmail) return BadRequest("Email is not correct");

                try
                {
                    if (loginModel.Email is not null && loginModel.Password is not null && isValidEmail && DifficultyCheck(loginModel.Password))
                        userId = await userRepository.AddUserAsync(loginModel.Email, loginModel.Password);
                    else
                        throw new Exception("The password is too unreliable.");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }
            redis.cache.Remove("users");
            return AcceptedAtAction(nameof(AddUser), userId);
        }


        [HttpGet(template: nameof(GetUsers))]
        [Authorize]
        public async Task<ActionResult<IEnumerable<GetUsersDto>?>> GetUsers()
        {
            if (redis.TryGetValue("users", out List<GetUsersDto>? cacheUsers) && cacheUsers is not null)
                return Accepted(nameof(GetUsers), cacheUsers);

            IEnumerable<GetUsersDto?>? users = await userRepository.GetUsersAsync();

            if (users is not null) redis.SetData("users", users);

            return Accepted(nameof(GetUsers), users);
        }


        [HttpDelete(template: nameof(DeleteUser))]
        [Authorize(Roles = nameof(UserRole.Administrator))]
        public async Task<ActionResult<Guid?>> DeleteUser(Guid userId)
        {
            Guid? deletedUserId = await userRepository.DeleteUserAsync(userId);
            if (deletedUserId is not null) redis.cache.Remove("users");
            return Accepted(nameof(DeleteUser), deletedUserId);
        }


        [HttpGet(template: nameof(GetCurrentUserId))]
        [Authorize]
        public ActionResult<Guid> GetCurrentUserId()
        {
            Guid currentUserId = CurrentUserId();

            return Accepted(nameof(GetCurrentUserId), currentUserId);
        }

        [HttpGet(template: nameof(CheckCurrentUserId))]
        [Authorize]
        public async Task<ActionResult<bool>> CheckCurrentUserId(Guid id)
        {
            bool existingUser = await userRepository.ExistingUserAsync(id);
            return Accepted(nameof(CheckCurrentUserId), existingUser);
        }


        private Guid CurrentUserId()
        {
            IEnumerable<Claim> userClaims = HttpContext.User.Claims;

            Guid currentUserId = new Guid(userClaims.FirstOrDefault(claim => claim.Type == ClaimTypes.PrimarySid)?.Value);

            return currentUserId;
        }



        private UserModel? GetCurrentUser()
        {
            ClaimsIdentity? identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity is null) return null;

            IEnumerable<Claim> userClaims = identity.Claims;

            return new UserModel
            {
                userId = new Guid(userClaims.FirstOrDefault(claim => claim.Type == ClaimTypes.PrimarySid)?.Value),
                UserEmail = userClaims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value,
                Role = (UserRole)Enum.Parse(typeof(UserRole), userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value),
            };
        }


        private bool DifficultyCheck(string password)
        {
            if (password.Length >= 7 &&
                password.Length <= 25 &&
                password.Any(ch => char.IsLower(ch)) &&
                password.Any(ch => char.IsUpper(ch)) &&
                password.Any(ch => char.IsLetter(ch)) &&
                password.Any(ch => char.IsDigit(ch)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }        
    }
}
