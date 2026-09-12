using System.Net.Http.Json;
using System.Text.Json;

var baseUrl = Environment.GetEnvironmentVariable("ELASTIC_URL") ?? "http://localhost:9200";
var indexName = "engineering-lab-es-001";
var refreshPolicy = "false";

using var http = new HttpClient { BaseAddress = new Uri(baseUrl) };

await DeleteIndexIfExistsAsync(http, indexName);
await CreateIndexAsync(http, indexName);

var document = new ProductDocument("sku-100", "Mechanical Keyboard", true);
var indexResponse = await http.PutAsJsonAsync($"/{indexName}/_doc/{document.Id}?refresh={refreshPolicy}", document);
Console.WriteLine($"WRITE_STATUS={(int)indexResponse.StatusCode}");
indexResponse.EnsureSuccessStatusCode();

var getResponse = await http.GetAsync($"/{indexName}/_doc/{document.Id}");
Console.WriteLine($"DIRECT_GET_STATUS={(int)getResponse.StatusCode}");

var immediateHits = await SearchByNameAsync(http, indexName, "Mechanical Keyboard");
Console.WriteLine($"IMMEDIATE_SEARCH_HITS={immediateHits}");

await Task.Delay(TimeSpan.FromSeconds(31));

var delayedHits = await SearchByNameAsync(http, indexName, "Mechanical Keyboard");
Console.WriteLine($"DELAYED_SEARCH_HITS={delayedHits}");

static async Task DeleteIndexIfExistsAsync(HttpClient http, string indexName)
{
    using var response = await http.DeleteAsync($"/{indexName}");
    if (response.IsSuccessStatusCode || (int)response.StatusCode == 404)
    {
        return;
    }

    response.EnsureSuccessStatusCode();
}

static async Task CreateIndexAsync(HttpClient http, string indexName)
{
    var body = new
    {
        settings = new Dictionary<string, object>
        {
            ["index.refresh_interval"] = "30s"
        },
        mappings = new
        {
            properties = new
            {
                name = new { type = "keyword" },
                published = new { type = "boolean" }
            }
        }
    };

    using var response = await http.PutAsJsonAsync($"/{indexName}", body);
    response.EnsureSuccessStatusCode();
}

static async Task<int> SearchByNameAsync(HttpClient http, string indexName, string name)
{
    var query = new
    {
        query = new
        {
            term = new
            {
                name = new { value = name }
            }
        }
    };

    using var response = await http.PostAsJsonAsync($"/{indexName}/_search", query);
    response.EnsureSuccessStatusCode();

    using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    return json.RootElement.GetProperty("hits").GetProperty("hits").GetArrayLength();
}

internal sealed record ProductDocument(string Id, string Name, bool Published);
