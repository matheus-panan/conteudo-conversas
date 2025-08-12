using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using painel_conversas.Models;
using System;

namespace painel_conversas
{
    public class ContactService
    {
        private readonly HttpClient _httpRequest;

        public ContactService(HttpClient httpClient)
        {
            _httpRequest = httpClient;
        }

        public async Task<List<ContactItem>> GetContacts()
        {
            try
            {
                var request = new HttpRequestMessage(
                    HttpMethod.Get,
                    "https://cbm-wap-babysuri-cb79660382-panan.azurewebsites.net/api/contacts"
                );

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "4962afae-009f-449b-a2be-8ccce74710ce");

                var response = await _httpRequest.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<ContactRootResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result?.Data?.Items ?? new List<ContactItem>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Deserialization Error: {ex.Message}");
                Console.WriteLine($"Path: {ex.Path}");
                return new List<ContactItem>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                return new List<ContactItem>();
            }
        }
    }
}