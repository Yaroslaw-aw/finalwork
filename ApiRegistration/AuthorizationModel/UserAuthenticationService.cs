
using ApiRegistration.Db;
using ApiRegistration.Dto;
using ApiRegistration.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ApiRegistration.AuthorizationModel
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        IUserRepository repository;
        private readonly IConfiguration config;
        UserContext context;

        public UserAuthenticationService(IUserRepository repository, IConfiguration config, UserContext context)
        {
            this.repository = repository;
            this.config = config;
            this.context = context;
        }

        private static UserRole RoleIdToUserRole(RoleId roleId)
        {
            if (roleId is RoleId.Admin) return UserRole.Administrator;

            return UserRole.User;
        }

        public async Task<string> AuthenticateAsync(LoginModel loginModel)
        {
            User? user = await repository.CheckUserAsync(loginModel.Email, loginModel.Password);

            UserModel userModel = new UserModel
            {
                UserEmail = loginModel.Email,
                Password = loginModel.Password,
                Role = RoleIdToUserRole(user.RoleId),
                userId = user.userId
            };

            string token = GenerateToken(userModel);

            return token;
        }

        public async Task<User> CheckUserAsync(LoginModel loginModel)
        {
            using (context)
            {
                User? user = await context.Users.FirstOrDefaultAsync(usr => usr.Email == loginModel.Email);

                if (user is null)
                {
                    throw new Exception($"{loginModel.Email} does not exist");
                }

                byte[]? bpassword = HashPassword(loginModel.Password, user.Salt);

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

        private string GenerateToken(UserModel user)
        {
            RsaSecurityKey? key = new RsaSecurityKey(RsaTools.GetPrivateKey());
            SigningCredentials? credentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha512Signature);

            Claim[]? claim = new[]
            {
                new Claim(ClaimTypes.PrimarySid, user.userId.ToString()),
                new Claim(ClaimTypes.Email, user.UserEmail),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            };

            JwtSecurityToken? token = new JwtSecurityToken(config["Jwt:Issuer"],
                config["Jwt:Audience"],
                claim,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
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
