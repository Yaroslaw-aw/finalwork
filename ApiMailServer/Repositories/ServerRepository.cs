using ApiMailServer.Db;
using Microsoft.EntityFrameworkCore;

namespace ApiMailServer.Repositories
{
    public class ServerRepository : IServerRepository
    {
        private readonly MessageContext context;
        private HttpClient client;

        public ServerRepository(MessageContext context)
        {
            this.context = context;
            client = new HttpClient();
        }

        public async Task<Guid?> WriteMessageAsync(Message message, Guid producerId)
        {
            using (context)
            {
                message.Status = MessageStatus.Sent;
                message.ProducerId = producerId;
                context.Messages.Add(message);
                await context.SaveChangesAsync();
                return message.Id;
            }
        }

        public async Task<IEnumerable<Message>?> GetMessagesAsync(Guid consumerId)
        {
            using (context)
            {
                IEnumerable<Message> messages = await context.Messages
                                                      .Where(message => message.ConsumerId == consumerId &&
                                                                        message.Status == MessageStatus.Sent).ToListAsync();

                foreach (var message in messages)
                {
                    message.Status = MessageStatus.Delivered;
                }

                await context.SaveChangesAsync();

                return messages;
            }
        }
    }
}
