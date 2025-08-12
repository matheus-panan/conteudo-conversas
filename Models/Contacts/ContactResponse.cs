using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace painel_conversas.Models
{
    public class ContactRootResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("data")]
        public ContactDataWrapper Data { get; set; }

        [JsonPropertyName("error")]
        public object Error { get; set; }
    }

    public class ContactDataWrapper
    {
        [JsonPropertyName("items")]
        public List<ContactItem> Items { get; set; }

        [JsonPropertyName("continuationToken")]
        public string ContinuationToken { get; set; }
    }

    public class ContactItem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("identificationDocument")]
        public string IdentificationDocument { get; set; }

        [JsonPropertyName("note")]
        public string Note { get; set; }

        [JsonPropertyName("dateCreate")]
        public string DateCreate { get; set; }

        [JsonPropertyName("lastActivity")]
        public string LastActivity { get; set; }

        [JsonPropertyName("lastMessageActivity")]
        public string LastMessageActivity { get; set; }
    }
}