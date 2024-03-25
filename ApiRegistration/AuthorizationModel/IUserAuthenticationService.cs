namespace ApiRegistration.AuthorizationModel
{
    public interface IUserAuthenticationService
    {
        Task<UserModel> AuthenticateAsync(LoginModel loginModel);
    }
}
