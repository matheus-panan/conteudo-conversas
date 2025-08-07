using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using painel_conversas.Models;

namespace painel_conversas.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpRequest;

        public ApiService(HttpClient httpClient)
        {
            _httpRequest = httpClient;
        }

        public async Task<List<Chat>> GetChat()
        {
            var requestIds = new HttpRequestMessage(
                HttpMethod.Get,
                "https://cbm-wap-babysuri-cb79660382-panan.azurewebsites.net/api/contacts"
            );
            // Envia o token no cabeçalho Authorization
            requestIds.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", "4962afae-009f-449b-a2be-8ccce74710ce");
            var responseIds = await _httpRequest.SendAsync(requestIds);
            responseIds.EnsureSuccessStatusCode();
            var contentIds = await responseIds.Content.ReadAsStringAsync();
            List<string> idList = new();
            /*var ids = JsonSerializer.Deserialize<ChatResponse>(contentIds, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });*/

            using (JsonDocument doc = JsonDocument.Parse(contentIds))
            {
                var root = doc.RootElement;

                if (root.TryGetProperty("data", out var data) &&
                    data.TryGetProperty("Items", out var items))
                {
                    // Verifica se Items é uma lista
                    if (items.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in items.EnumerateArray())
                        {
                            if (item.TryGetProperty("data", out var dataItem) &&
                                dataItem.TryGetProperty("id", out var idProp))
                            {
                                idList.Add(idProp.GetString());
                            }
                        }
                    }
                    // Caso seja um único objeto e não um array (igual ao exemplo que você mandou antes)
                    else if (items.ValueKind == JsonValueKind.Object)
                    {
                        if (items.TryGetProperty("data", out var dataItem) &&
                            dataItem.TryGetProperty("id", out var idProp))
                        {
                            idList.Add(idProp.GetString());
                        }
                    }
                }
            }
            
            List<Chat> allChats = new();
            foreach (var id in idList)
            {
                var requestChat = new HttpRequestMessage(
                    HttpMethod.Get,
                    $"https://cbm-wap-babysuri-cb79660382-panan.azurewebsites.net/api/contacts/{id}/messages"
                );
                // Envia o token no cabeçalho Authorization
                requestChat.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", "4962afae-009f-449b-a2be-8ccce74710ce");
                var responseChat = await _httpRequest.SendAsync(requestChat);
                responseChat.EnsureSuccessStatusCode();

                var content = await responseChat.Content.ReadAsStringAsync();

                // Desserializa como UM ChatResponse (não uma lista!)
                var result = JsonSerializer.Deserialize<ChatResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result == null)
                    //return new List<Chat>();
                    continue;
                allChats.AddRange(result.Data);
            }
            return allChats;
        }
    }
}
