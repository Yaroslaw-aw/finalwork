
namespace ApiRegistration.AuthorizationModel
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        public Task<UserModel> AuthenticateAsync(LoginModel loginModel)
        {
            throw new NotImplementedException();
        }
    }
}
