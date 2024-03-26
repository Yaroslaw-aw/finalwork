using ApiRegistration.AuthorizationModel;
using ApiRegistration.Client;
using ApiRegistration.Db;
using ApiRegistration.Dto;
using ApiRegistration.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ApiRegistration.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly IMemoryCache cache;
        private readonly IClientServer clientServer;

        public UserController(IUserRepository userRepository, IMemoryCache cache, IClientServer clientServer)
        {
            this.userRepository = userRepository;
            this.cache = cache;
            this.clientServer = clientServer;
        }

        [AllowAnonymous]
        [HttpPost(template: "AddUser")]
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
            cache.Remove("users");
            return AcceptedAtAction(nameof(AddUser), userId);
        }


        [HttpGet(template: "GetUsers")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<GetUsersDto>?>> GetUsersAsync()
        {
            if (cache.TryGetValue("users", out List<GetUsersDto>? cacheUsers) && cacheUsers is not null)
                return Accepted("GetUsers", cacheUsers);

            IEnumerable<GetUsersDto>? users = await userRepository.GetUsersAsync();

            if (users is not null) cache.Set("users", users, TimeSpan.FromMinutes(10));

            return Accepted("GetUsers", users);
        }


        [HttpDelete(template: "DeleteUser")]
        [Authorize(Roles = nameof(UserRole.Administrator))]
        public async Task<ActionResult<Guid?>> DeleteUser(Guid userId)
        {
            Guid? deletedUserId = await userRepository.DeleteUserAsync(userId);
            if (deletedUserId is not null) cache.Remove("users");
            return Accepted(nameof(DeleteUser), deletedUserId);
        }



        [HttpPost(template: "WriteMessage")]
        [Authorize]
        public async Task<ActionResult<string?>> WriteMessage(MessagePostDto message)
        {
            Guid producerId = CurrentUserId();
            Guid? consumerId = message.ConsumerId;
            string? content = message.Content;

            string? messageId = await clientServer.WriteMessage(content, consumerId, producerId);

            return messageId;
        }

        [HttpGet(template: "GetMessages")]
        [Authorize]
        public async Task<ActionResult<string?>> GetMessages()
        {
            Guid id = CurrentUserId();

            string? messages = await clientServer.GetMessagesAsync(id);

            IEnumerable<MessageGetDto>? mess = JsonSerializer.Deserialize<IEnumerable<MessageGetDto>>(messages);

            return Accepted(nameof(GetMessages), mess);
        }


        [HttpGet(template: "GetCurrentUserId")]
        [Authorize]
        public async Task<ActionResult<Guid?>> GetCurrentUserId()
        {
            Guid currentUserId = CurrentUserId();

            return Accepted(nameof(GetCurrentUserId), currentUserId);
        }


        private Guid CurrentUserId()
        {
            ClaimsIdentity? identity = HttpContext.User.Identity as ClaimsIdentity;

            IEnumerable<Claim> userClaims = identity.Claims;

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
