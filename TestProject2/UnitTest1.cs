using ApiRegistration.AuthorizationModel;
using ApiRegistration.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace TestProject2
{
    public class UnitTest1
    {

        [Fact]
        public async void TestMethodLogin()
        {
            LoginModel login = new LoginModel { Email = "Vasya@mail.ru", Password = "Admin123!" };
            var mockUserService = new Mock<IUserAuthenticationService>();
            mockUserService.Setup(service => service.AuthenticateAsync(login)).Returns(Task.Run(() => "token"));
            var controller = new LoginController(mockUserService.Object);

            Task<ActionResult<string>> result = controller.Login(login);

            Assert.IsType<ActionResult<string>>(await result);
        }
    }
}