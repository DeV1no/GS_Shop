using System.Net.Http.Json;
using FluentAssertions;
using GS_Shop_UserManagement.Application.DTOs.User;

namespace GS_Shop_UserManagement_Integration_Test
{
    public class UserControllerTest
    {
        [Fact]
        public async Task Register_AddUser()
        {
            // Arrange
            var application = new GsShopUserManagementApplicationFactory();
            var request = new RegisterUserDto
            {
                Id = 0,
                Email = "test12@test16.com",
                FirstName = "test",
                LastName = "test",
                UserName = "testgrammms",
                Password = "T3$t12345@@",
            };
            var client = application.CreateClient();

            // Act
            var response = await client.PostAsJsonAsync("/api/User/register", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var registerResponse = await response.Content.ReadFromJsonAsync<int>();
            registerResponse.Should().Be(1);
        }
    }
}