using Microsoft.AspNetCore.Identity;

namespace ApiRegistration.AuthorizationModel
{
    public class UserModel
    {
        public Guid userId {  get; set; }
        public string? UserEmail { get; set; }
        public string? Password { get; set; }
        public UserRole? Role { get; set; }
    }
}
