
using ApiRegistration.AuthorizationModel;
using ApiRegistration.Dto;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace ApiRegistration.Client
{
    public class ClientServer : IClientServer
    {
        private readonly HttpClient client = new HttpClient();

        public async Task<string?> GetMessagesAsync(Guid consumerId)
        {
            

            using HttpResponseMessage response = await client.GetAsync($"http://localhost:5210/Server/GetMessages?consumerId={consumerId.ToString()}");

            response.EnsureSuccessStatusCode();

            string? result = await response.Content.ReadAsStringAsync();

            return result;
        }

        public async Task<string?> WriteMessageAsync(string? message, Guid? consumerId, Guid? producerId)
        {
            SendMessageDto messageDto = new SendMessageDto() { Content = message, ConsumerId = consumerId, ProducerId = producerId };

            HttpResponseMessage response = await client.PostAsJsonAsync("http://localhost:5210/Server/WriteMessage", messageDto);
            //HttpMessageHandler handler = await client.PostAsync("http://localhost:5210/Server/WriteMessage", messageDto);
            //HttpContent content = response.Content;
            //client.DefaultRequestHeaders

            response.EnsureSuccessStatusCode();

            string? responseBody = await response.Content.ReadAsStringAsync();

            if (responseBody is null)
                return null;
            else
                return responseBody;
        }



        //public ClientServer(string authSid)
        //{
        //    client = new HttpClient();

        //    client.BaseAddress = new Uri("http://localhost:5210/Server/WriteMessage");

        //    client.DefaultRequestHeaders.Add("token", $"auth.sid={authSid}");

        //    JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        //    {
        //        PropertyNameCaseInsensitive = true
        //    };

        //}

        public bool IsAuthSidExpired(string token)
        {
            
            if (token.Length < 30)
                throw new Exception($"Некорректный auth.Sid");

            using (client)
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage
                {
                    Headers = { { "Authorization", $"Bearer {token}" } },
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("http://localhost:5210/Server/WriteMessage"),
                    //Content = new StringContent(JsonSerializer.Serialize(new SendMessageDto() { Content = message, ConsumerId = consumerId, ProducerId = producerId }), encoding: Encoding.UTF8, "application/json")
                };
                HttpResponseMessage response = client.Send(httpRequestMessage);

                return response.StatusCode != HttpStatusCode.OK;
            }
        }

    }
}
