using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using painel_conversas.Models;
using painel_conversas.Models.Pagination;
using System;
using System.Linq;

namespace painel_conversas
{
    public class ContactService
    {
        private readonly HttpClient _httpRequest;
        private static List<ContactItem> _cachedContacts = new List<ContactItem>();
        private static DateTime _lastCacheUpdate = DateTime.MinValue;
        private static readonly TimeSpan CacheValidTime = TimeSpan.FromMinutes(5); // Cache por 5 minutos

        public ContactService(HttpClient httpClient)
        {
            _httpRequest = httpClient;
        }

        // Método principal otimizado com cache
        public async Task<List<ContactItem>> GetContacts()
        {
            try
            {
                // Verificar se o cache ainda é válido
                if (_cachedContacts.Any() && DateTime.Now - _lastCacheUpdate < CacheValidTime)
                {
                    Console.WriteLine("Usando contatos do cache...");
                    return _cachedContacts;
                }

                Console.WriteLine("Buscando contatos da API...");

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

                var contacts = result?.Data?.Items ?? new List<ContactItem>();

                // Atualizar cache
                _cachedContacts = contacts;
                _lastCacheUpdate = DateTime.Now;

                Console.WriteLine($"Carregados {contacts.Count} contatos da API");

                return contacts;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Deserialization Error: {ex.Message}");
                Console.WriteLine($"Path: {ex.Path}");
                return _cachedContacts; // Retorna cache em caso de erro
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                return _cachedContacts; // Retorna cache em caso de erro
            }
        }

        // Método com paginação real
        public async Task<PagedResult<ContactItem>> GetContactsPaged(int page = 1, int pageSize = 50)
        {
            try
            {
                var allContacts = await GetContacts();
                var pagedResult = PagedResult<ContactItem>.Create(allContacts, page, pageSize);
                
                return pagedResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting paged contacts: {ex.Message}");
                return new PagedResult<ContactItem>
                {
                    Items = new List<ContactItem>(),
                    CurrentPage = page,
                    TotalPages = 0,
                    PageSize = pageSize,
                    TotalItems = 0
                };
            }
        }

        // Método otimizado para buscar um contato específico
        public async Task<ContactItem> GetContactById(string contactId)
        {
            try
            {
                // Primeiro tentar no cache
                if (_cachedContacts.Any())
                {
                    var cachedContact = _cachedContacts.FirstOrDefault(c => c.Id == contactId);
                    if (cachedContact != null)
                    {
                        return cachedContact;
                    }
                }

                // Se não achou no cache, buscar todos os contatos
                var contacts = await GetContacts();
                var contact = contacts.FirstOrDefault(c => c.Id == contactId);
                
                if (contact != null)
                {
                    return contact;
                }

                Console.WriteLine($"Contact {contactId} not found");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting contact by ID {contactId}: {ex.Message}");
                return null;
            }
        }

        // Método para limpar cache (útil para testes ou atualizações forçadas)
        public void ClearCache()
        {
            _cachedContacts.Clear();
            _lastCacheUpdate = DateTime.MinValue;
            Console.WriteLine("Cache de contatos limpo");
        }

        // Método para obter estatísticas do cache
        public (int Count, DateTime LastUpdate, bool IsValid) GetCacheStats()
        {
            var isValid = _cachedContacts.Any() && DateTime.Now - _lastCacheUpdate < CacheValidTime;
            return (_cachedContacts.Count, _lastCacheUpdate, isValid);
        }
    }
}