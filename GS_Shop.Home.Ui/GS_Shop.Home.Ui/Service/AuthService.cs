using System.Net.Http.Json;

using GS_Shop.Home.Ui.Models;

namespace GS_Shop.Home.Ui.Service;

public class AuthService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest loginRequest)
    {
        var httpClient = _httpClientFactory.CreateClient("ApiClient"); // Get the correct HttpClient

        var response = await httpClient.PostAsJsonAsync("api/User/login", loginRequest);
        
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<LoginResponse>();
        }
        else
        {
            return new LoginResponse { Message = "Login failed" };
        }
    }
}
