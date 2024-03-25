using ApiMailServer.Db;

namespace ApiMailServer.Repositories
{
    public interface IServerRepository
    {
        public Task<IEnumerable<Message>?> GetMessagesAsync(Guid consumerId);
        public Task<Guid?> WtiteMessageAsync(Message message);
    }
}
