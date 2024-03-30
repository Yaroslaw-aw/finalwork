namespace ApiRegistration.AuthorizationModel
{
    public interface IUserAuthenticationService
    {
        Task<string> AuthenticateAsync(LoginModel loginModel);
    }
}
