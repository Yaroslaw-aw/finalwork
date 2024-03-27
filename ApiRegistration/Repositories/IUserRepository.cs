using ApiRegistration.Db;
using ApiRegistration.Dto;
using Microsoft.AspNetCore.Mvc;

namespace ApiRegistration.Repositories
{
    public interface IUserRepository
    {
        public Task<Guid?> AddUserAsync(string email, string password);
        public Task<User?> CheckUserAsync(string email, string password);
        public Task<Guid?> DeleteUserAsync(Guid deleteUserId);
        public Task<IEnumerable<GetUsersDto>?> GetUsersAsync();
        public Task<bool> ExistingUserAsync(Guid userId);
    }
}
