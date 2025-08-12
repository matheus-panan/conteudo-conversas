using System.Collections.Generic;
using System.Text.Json.Serialization;
using painel_conversas.Models;

public class RootResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("data")]
    public DataWrapper Data { get; set; }
}

public class DataWrapper
{
    [JsonPropertyName("Items")]
    public ItemsWrapper Items { get; set; }

    [JsonPropertyName("ContinuationToken")]
    public string ContinuationToken { get; set; }
}

public class ItemsWrapper
{
    [JsonPropertyName("data")]
    [JsonConverter(typeof(SingleOrArrayConverter<Contact>))]
    public List<Contact> Data { get; set; }
}


public class Contact
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
}