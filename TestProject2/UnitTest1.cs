using ApiRegistration.AuthorizationModel;
using ApiRegistration.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace TestProject2
{
    public class UnitTest1
    {

        [Fact]
        public void TestMethodLogin()
        {
            // service =>  service.AuthenticateAsync(login)).Returns("token")
            LoginModel login = new LoginModel { Email = "Vasya@mail.ru", Password = "Admin123!" };
            var mockUserService = new Mock<IUserAuthenticationService>();
            mockUserService.Setup(service => service.AuthenticateAsync(login)).Returns(Task.Run(() =>"token"));
            var controller = new LoginController(mockUserService.Object);


            var result = controller.Login(login);


            var objResult = Assert.IsType<OkObjectResult>(result);
            var res = Assert.IsType<string>(objResult.Value);
            Assert.Equal("token", res);
        }


    }
}