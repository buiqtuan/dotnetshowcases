using PlatformService.Dtos;
using System.Text.Json;
using System.Text;

namespace PlatformService.SyncDataServices.Http;

public class HttpCommandDataClient : ICommandDataClient
{
    private readonly HttpClient _httpClient;

    private readonly IConfiguration _configuration;

    public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task SendPlatformToCommand(PlatformCreateDto platform)
    {
        var httpContent = new StringContent(
            JsonSerializer.Serialize(platform),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync($"{_configuration["CommandService"]}", httpContent);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("--> Sync POST to Command Service was OK!");
        }
        else
        {
            Console.WriteLine("--> Sync POST to Command Service was NOT OK!");
        }
    }
}