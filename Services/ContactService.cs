using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using painel_conversas.Models;


namespace painel_conversas
{
    public class ContactService
    {
        private readonly HttpClient _httpRequest;

        public ContactService(HttpClient httpClient)
        {
            _httpRequest = httpClient;
        }

        public async Task<List<string>> GetContacts()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "https://cbm-wap-babysuri-cb79660382-panan.azurewebsites.net/api/contacts"
            );

            // Se precisar de autenticação por token
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "4962afae-009f-449b-a2be-8ccce74710ce");

            var response = await _httpRequest.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<RootResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Extrai apenas os IDs (considerando lista)
            var ids = result?.Data?.Items?.Data?.ConvertAll(c => c.Id) ?? new List<string>();

            return ids;
        }
    }
}