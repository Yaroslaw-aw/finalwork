using ApiMailServer.Db;
using ApiMailServer.Dto;
using ApiMailServer.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost(template: "WriteMessge")]
        //[Authorize]
        public async Task<ActionResult<Guid?>> WriteMessge(MessageDto messageDto)
        {
            Message message = mapper.Map<Message>(messageDto);

            Guid? newMessageId = await repository.WtiteMessageAsync(message);

            return CreatedAtAction(nameof(WriteMessge), newMessageId);
        }

        [HttpGet(template: "GetMessages")]
        //[Authorize]
        public async Task<ActionResult<string>> GetMessages(Guid consumerId)
        {
            IEnumerable<Message>? messages = await repository.GetMessagesAsync(consumerId);

            IEnumerable<MessagesSentDto>? messagesDto = new MessagesSentDto[messages.Count()];

            messagesDto = mapper.Map(messages, messagesDto);

            string send = JsonSerializer.Serialize(messagesDto);

            return Accepted(nameof(GetMessages), send);
        }
    }
}
