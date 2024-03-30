using ApiMailServer.AuthorizationModel;
using ApiMailServer.Db;
using ApiMailServer.Dto;
using ApiMailServer.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiMailServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServerController : Controller
    {
        private readonly IServerRepository repository;
        private readonly IMapper mapper;

        public ServerController(IServerRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        [HttpPost(template: "WriteMessage")]
        [Authorize]
        public async Task<ActionResult<Guid?>> WriteMessage([FromBody] MessageDto messageDto)
        {
            Message message = mapper.Map<Message>(messageDto);

            UserModel? producer = GetCurrentUser();

            message.ProducerEmail = producer?.UserEmail;

            Guid producerId = CurrentUserId();

            Guid? newMessageId = await repository.WriteMessageAsync(message, producerId);

            return CreatedAtAction(nameof(WriteMessage), newMessageId);
        }

        [HttpGet(template: "GetMessages")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MessagesSentDto>?>> GetMessages()
        {
            Guid consumerId = CurrentUserId();
            IEnumerable<Message>? messages = await repository.GetMessagesAsync(consumerId);

            if (messages is null) return Accepted(nameof(GetMessages), "There is no new messages for you");

            IEnumerable<MessagesSentDto>? messagesDto = new MessagesSentDto[messages.Count()];            

            messagesDto = mapper.Map(messages, messagesDto);

            return Accepted(nameof(GetMessages), messagesDto);
        }

        private Guid CurrentUserId()
        {
            ClaimsIdentity? identity = HttpContext.User.Identity as ClaimsIdentity;

            IEnumerable<Claim>? userClaims = identity?.Claims;

            string? userId = userClaims?.FirstOrDefault(claim => claim.Type == ClaimTypes.PrimarySid)?.Value;

            if (string.IsNullOrWhiteSpace(userId)) return Guid.Empty;

            Guid currentUserId = Guid.Parse(userId);
            return currentUserId;
        }

        private UserModel? GetCurrentUser()
        {
            ClaimsIdentity? identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity is not null)
            {
                IEnumerable<Claim> userClaims = identity.Claims;

                // достаём Id пользователя из клайма
                Guid? userId = null;
                string? userIdClaim =    userClaims?.FirstOrDefault(claim => claim.Type == ClaimTypes.PrimarySid)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) == false) userId = Guid.Parse(userIdClaim);

                // достаём почту пользователя из клайма
                string? userEmailClaim = userClaims?.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value;

                // достаём роль пользователя из клайма
                UserRole? userRole = null;
                string? userRoleClaim =  userClaims?.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
                if (string.IsNullOrEmpty(userRoleClaim))                
                    userRole = null;                
                else                
                    userRole = (UserRole)Enum.Parse(typeof(UserRole), userRoleClaim);

                return new UserModel
                {
                    userId = userId,
                    UserEmail = userEmailClaim,
                    Role = userRole,
                };
            }
            return null;
        }
    }
}