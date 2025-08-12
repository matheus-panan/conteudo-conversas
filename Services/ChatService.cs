using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using painel_conversas.Models;
using System.Linq;
using System;

namespace painel_conversas
{
    public class ChatService
    {
        private readonly HttpClient _httpRequest;
        private readonly ContactService _contactService;

        public ChatService(HttpClient httpClient, ContactService contactService)
        {
            _httpRequest = httpClient;
            _contactService = contactService;
        }

        // Método original - busca conversas de um contato específico
        public async Task<List<Chat>> GetChatByContact(string contactId)
        {
            try
            {
                var request = new HttpRequestMessage(
                    HttpMethod.Get,
                    $"https://cbm-wap-babysuri-cb79660382-panan.azurewebsites.net/api/contacts/{contactId}/messages"
                );

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
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting chat for contact {contactId}: {ex.Message}");
                return new List<Chat>();
            }
        }

        // Novo método - busca conversas de todos os contatos
        public async Task<List<Chat>> GetAllChats()
        {
            try
            {
                // Primeiro, busca todos os contatos
                var contacts = await _contactService.GetContacts();
                var allChats = new List<Chat>();

                // Para cada contato, busca suas conversas
                var tasks = contacts.Select(async contact =>
                {
                    try
                    {
                        var chats = await GetChatByContact(contact.Id);
                        return chats;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error getting chats for contact {contact.Id}: {ex.Message}");
                        return new List<Chat>();
                    }
                });

                var results = await Task.WhenAll(tasks);
                
                // Combina todas as conversas em uma única lista
                foreach (var chatList in results)
                {
                    allChats.AddRange(chatList);
                }

                // Ordena por data de criação (mais recentes primeiro)
                return allChats.OrderByDescending(c => c.CreatedAt).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all chats: {ex.Message}");
                return new List<Chat>();
            }
        }

        // Método para compatibilidade com código existente
        public async Task<List<Chat>> GetChat()
        {
            return await GetAllChats();
        }
    }
}