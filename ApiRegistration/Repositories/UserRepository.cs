using ApiRegistration.Db;
using ApiRegistration.Dto;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ApiRegistration.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserContext context;
        private readonly IMapper mapper;

        public UserRepository(UserContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<Guid?> AddUserAsync(string email, string password)
        {
            RoleId roleId = new RoleId();

            using (context)
            {
                if (context.Users.Count() == 0)
                    roleId = RoleId.Admin;
                else
                    roleId = RoleId.User;

                byte[] salt = GenerateSalt();
                byte[] hashedPassword = HashPassword(password, salt);

                User user = new User
                {
                    Email = email,
                    RoleId = roleId,
                    Password = hashedPassword,
                    Salt = salt
                };

                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                return user.userId;
            }
        }

        public async Task<User?> CheckUserAsync(string email, string password)
        {
            using (context)
            {
                User? user = await context.Users.FirstOrDefaultAsync(usr => usr.Email == email);

                if (user is null)
                {
                    throw new Exception($"{email} does not exist");
                }
                
                byte[]? bpassword = HashPassword(password, user.Salt);

                if (user.Password.SequenceEqual(bpassword))
                {
                    return user;
                }
                else
                {
                    throw new Exception("Wrong email or password");
                }
            }
        }        

        public async Task<Guid?> DeleteUserAsync(Guid deleteUserId)
        {
            using (context)
            {
                User? user = await context.Users.FirstOrDefaultAsync(usr => usr.userId == deleteUserId);

                if (user is null)
                {
                    throw new Exception($"{deleteUserId} does not exist");
                }
                else if (user.RoleId is RoleId.Admin)
                {
                    throw new Exception($"It's not possible to delete Administrator");
                }

                context.Users.Remove(user);

                await context.SaveChangesAsync();

                return user.userId;
            }
        }


        public async Task<IEnumerable<GetUsersDto>?> GetUsersAsync()
        {
            IEnumerable<User>? users = null;
            IEnumerable<GetUsersDto>? usersDto = null;

            using (context) users = await context.Users.AsNoTracking().ToListAsync();

            if (users is null) return null;

            usersDto = mapper.Map(users, usersDto);

            return usersDto;
        }

        public async Task<bool> ExistingUserAsync(Guid userId)
        {
            return await context.Users.AnyAsync(usr => usr.userId == userId);
        }


        private byte[] GenerateSalt()
        {
            byte[] salt = new byte[32];
            using (RandomNumberGenerator? rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        private byte[] HashPassword(string password, byte[] salt)
        {
            using (SHA512? sha512 = SHA512.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] data = passwordBytes.Concat(salt).ToArray();
                return sha512.ComputeHash(data);
            }
        }
    }
}