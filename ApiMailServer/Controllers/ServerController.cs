using ApiMailServer.AuthorizationModel;
using ApiMailServer.Db;
using ApiMailServer.Dto;
using ApiMailServer.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

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

            Guid producerId = CurrentUserId();

            Guid? newMessageId = await repository.WriteMessageAsync(message, producerId);

            return CreatedAtAction(nameof(WriteMessage), newMessageId);
        }

        [HttpGet(template: "GetMessages")]
        [Authorize]
        public async Task<ActionResult<string>> GetMessages()
        {
            Guid consumerId = CurrentUserId();
            IEnumerable<Message>? messages = await repository.GetMessagesAsync(consumerId);

            IEnumerable<MessagesSentDto>? messagesDto = new MessagesSentDto[messages.Count()];

            messagesDto = mapper.Map(messages, messagesDto);

            string send = JsonSerializer.Serialize(messagesDto);

            return Accepted(nameof(GetMessages), send);
        }

        private Guid CurrentUserId()
        {
            ClaimsIdentity? identity = HttpContext.User.Identity as ClaimsIdentity;

            IEnumerable<Claim> userClaims = identity.Claims;

            Guid currentUserId = new Guid(userClaims.FirstOrDefault(claim => claim.Type == ClaimTypes.PrimarySid)?.Value);

            return currentUserId;
        }

        private UserModel GetCurrentUser()
        {
            ClaimsIdentity? identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity is not null)
            {
                IEnumerable<Claim> userClaims = identity.Claims;
                return new UserModel
                {
                    userId = new Guid(userClaims.FirstOrDefault(claim => claim.Type == ClaimTypes.PrimarySid)?.Value),
                    UserEmail = userClaims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value,
                    Role = (UserRole)Enum.Parse(typeof(UserRole), userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value),
                };
            }
            return null;
        }
    }
}
