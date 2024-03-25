
using ApiRegistration.AuthorizationModel;
using ApiRegistration.Dto;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;

namespace ApiRegistration.Client
{
    public class ClientServer : IClientServer
    {
        HttpClient client = new HttpClient();

        public async Task<string?> GetMessagesAsync(Guid consumerId)
        {
            using HttpResponseMessage response = await client.GetAsync($"http://mailserverhost:8080/Server/GetMessages?consumerId={consumerId.ToString()}");

            response.EnsureSuccessStatusCode();

            string? result = await response.Content.ReadAsStringAsync();

            return result;
        }

        public async Task<string?> WriteMessage(string? message, Guid? consumerId, Guid? producerId)
        {
            SendMessageDto messageDto = new SendMessageDto() { Content = message, ConsumerId = consumerId, ProducerId = producerId };

            HttpResponseMessage response = await client.PostAsJsonAsync("http://mailserverhost:8080/Server/WriteMessge", messageDto);

            response.EnsureSuccessStatusCode();

            string? responseBody = await response.Content.ReadAsStringAsync();

            if (responseBody is null)
                return null;
            else
                return responseBody;
        }



    }
}
