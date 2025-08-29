using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using painel_conversas.Models;
using painel_conversas.Models.Pagination;
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

        // Método otimizado - busca conversas paginadas de um contato específico
        public async Task<PagedResult<Chat>> GetChatByContactPaged(string contactId, int page = 1, int pageSize = 50)
        {
            try
            {
                // Buscar todas as conversas do contato (isso ainda precisa ser feito por limitação da API)
                var allChats = await GetChatByContact(contactId);
                
                // Aplicar paginação em memória
                var pagedResult = PagedResult<Chat>.Create(allChats, page, pageSize);
                
                return pagedResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting paged chat for contact {contactId}: {ex.Message}");
                return new PagedResult<Chat>
                {
                    Items = new List<Chat>(),
                    CurrentPage = page,
                    TotalPages = 0,
                    PageSize = pageSize,
                    TotalItems = 0
                };
            }
        }

        // Método OTIMIZADO - busca conversas de múltiplos contatos com paginação inteligente
        public async Task<PagedResult<Chat>> GetAllChatsPaged(int page = 1, int pageSize = 50)
        {
            try
            {
                // Primeiro, busca todos os contatos
                var contacts = await _contactService.GetContacts();
                
                if (!contacts.Any())
                {
                    return new PagedResult<Chat>
                    {
                        Items = new List<Chat>(),
                        CurrentPage = page,
                        TotalPages = 0,
                        PageSize = pageSize,
                        TotalItems = 0
                    };
                }

                // **OTIMIZAÇÃO CHAVE**: Processar contatos em lotes menores para evitar sobrecarga
                var allChats = new List<Chat>();
                var batchSize = 5; // Processa 5 contatos por vez
                var contactBatches = contacts.Batch(batchSize);

                foreach (var batch in contactBatches)
                {
                    // Processar lote em paralelo (máximo 5 contatos por vez)
                    var tasks = batch.Select(async contact =>
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
                    
                    // Combinar resultados do lote atual
                    foreach (var chatList in results)
                    {
                        allChats.AddRange(chatList);
                    }

                    // **IMPORTANTE**: Pequena pausa entre lotes para não sobrecarregar a API
                    await Task.Delay(100);
                }

                // Ordenar por data de criação (mais recentes primeiro)
                allChats = allChats.OrderByDescending(c => c.CreatedAt).ToList();

                // Aplicar paginação
                var pagedResult = PagedResult<Chat>.Create(allChats, page, pageSize);
                
                return pagedResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all paged chats: {ex.Message}");
                return new PagedResult<Chat>
                {
                    Items = new List<Chat>(),
                    CurrentPage = page,
                    TotalPages = 0,
                    PageSize = pageSize,
                    TotalItems = 0
                };
            }
        }

        // Método original mantido para compatibilidade
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

                var contact = await _contactService.GetContactById(contactId);
                var chats = result.Data;

                await EnrichChatsWithSenderInfo(chats, contact);

                return chats;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting chat for contact {contactId}: {ex.Message}");
                return new List<Chat>();
            }
        }

        // Método para buscar contatos de forma otimizada com limite
        public async Task<PagedResult<Chat>> GetAllChatsOptimized(int page = 1, int pageSize = 50, int maxContacts = 20)
        {
            try
            {
                // Buscar contatos com limite para evitar sobrecarga inicial
                var allContacts = await _contactService.GetContacts();
                var limitedContacts = allContacts.Take(maxContacts).ToList();

                Console.WriteLine($"Processando {limitedContacts.Count} contatos de {allContacts.Count} totais para melhor performance");

                var allChats = new List<Chat>();
                var processed = 0;

                // Processar em lotes pequenos com feedback
                foreach (var contact in limitedContacts)
                {
                    try
                    {
                        var chats = await GetChatByContact(contact.Id);
                        allChats.AddRange(chats);
                        processed++;

                        // Log de progresso
                        if (processed % 5 == 0)
                        {
                            Console.WriteLine($"Processados {processed}/{limitedContacts.Count} contatos...");
                        }

                        // Pausa para não sobrecarregar
                        await Task.Delay(50);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing contact {contact.Id}: {ex.Message}");
                    }
                }

                // Ordenar e paginar
                allChats = allChats.OrderByDescending(c => c.CreatedAt).ToList();
                var pagedResult = PagedResult<Chat>.Create(allChats, page, pageSize);
                
                Console.WriteLine($"Total de mensagens encontradas: {allChats.Count}");
                
                return pagedResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in optimized chat retrieval: {ex.Message}");
                return new PagedResult<Chat>
                {
                    Items = new List<Chat>(),
                    CurrentPage = page,
                    TotalPages = 0,
                    PageSize = pageSize,
                    TotalItems = 0
                };
            }
        }

        // Método auxiliar para enriquecer chats com informações do remetente
        private async Task EnrichChatsWithSenderInfo(List<Chat> chats, ContactItem contact)
        {
            foreach (var chat in chats)
            {
                if (contact != null && chat.SenderId == contact.Id)
                {
                    chat.SenderName = contact.Name ?? "Cliente";
                    chat.SenderType = "Cliente";
                }
                else if (chat.Origin == "bot" || chat.Origin == "system")
                {
                    chat.SenderName = "Sistema/Bot";
                    chat.SenderType = "Sistema";
                }
                else
                {
                    chat.SenderName = "Atendente";
                    chat.SenderType = "Atendente";
                }
            }
        }

        // Método para compatibilidade com código existente
        public async Task<List<Chat>> GetAllChats()
        {
            var pagedResult = await GetAllChatsOptimized(1, 1000, 10); // Máximo 10 contatos para não travar
            return pagedResult.Items;
        }
    }

    // Extension method para criar lotes
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int size)
        {
            T[] bucket = null;
            var count = 0;

            foreach (var item in source)
            {
                if (bucket == null)
                    bucket = new T[size];

                bucket[count++] = item;

                if (count != size)
                    continue;

                yield return bucket.Select(x => x);

                bucket = null;
                count = 0;
            }

            // Return the last bucket with all remaining elements
            if (bucket != null && count > 0)
            {
                yield return bucket.Take(count);
            }
        }
    }
}