namespace ApiMailServer.AuthorizationModel
{
    public class UserModel
    {
        public Guid? userId { get; set; }
        public string? UserEmail { get; set; }
        public string? Password { get; set; }
        public UserRole? Role { get; set; }
    }
}
