using ApiRegistration.Dto;
using Microsoft.AspNetCore.Mvc;

namespace ApiRegistration.Client
{
    public interface IClientServer
    {
        public Task<string?> WriteMessageAsync(string? message, Guid? consumerId, Guid? producerId);
        Task<string?> GetMessagesAsync(Guid consumerId);
    }
}
