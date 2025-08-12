using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using painel_conversas.Models;


namespace painel_conversas
{
    public class ChatService
    {
        private readonly HttpClient _httpRequest;

        public ChatService(HttpClient httpClient)
        {
            _httpRequest = httpClient;
        }
        
        
        public async Task<List<Chat>> GetChat()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "https://cbm-wap-babysuri-cb79660382-panan.azurewebsites.net/api/contacts/wp741920215664094:5567984497349/messages"
            );

            // Envia o token no cabeçalho Authorization
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "4962afae-009f-449b-a2be-8ccce74710ce");

            var response = await _httpRequest.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            
            var result = JsonSerializer.Deserialize<ChatResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result == null || result.Data == null)
                return new List<Chat>();

            return result.Data;
        }
    }
}