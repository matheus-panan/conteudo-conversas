using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using painel_conversas.Models;
using System;
using System.Linq;

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

        public async Task<ContactItem> GetContactById(string contactId)
        {
            try
            {
                // Primeiro tenta buscar todos os contatos e encontrar o específico
                var contacts = await GetContacts();
                var contact = contacts.FirstOrDefault(c => c.Id == contactId);
                
                if (contact != null)
                {
                    return contact;
                }

                // Se não encontrou na lista, tenta buscar diretamente pela API (se houver endpoint específico)
                Console.WriteLine($"Contact {contactId} not found in contacts list");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting contact by ID {contactId}: {ex.Message}");
                return null;
            }
        }
    }
}