using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace RaceDayWeb.Services
{
    public class ApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _baseUrl;

        public ApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _baseUrl = configuration["ApiSettings:BaseUrl"];
        }

        // Helper to create an HttpClient that passes the User ID and Role as Headers
        private HttpClient GetClient()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_baseUrl);

            // Read from MVC Session and send as custom headers to the API
            var userId = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
            var role = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");

            if (userId.HasValue && !string.IsNullOrWhiteSpace(role))
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("X-User-Id", userId.Value.ToString());
                client.DefaultRequestHeaders.TryAddWithoutValidation("X-User-Role", role);
            }

            return client;
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            var client = GetClient();
            var response = await client.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return default;
        }

        public async Task<HttpResponseMessage> PostAsync(string endpoint, object data)
        {
            var client = GetClient();
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await client.PostAsync(endpoint, content);
        }

        public async Task<HttpResponseMessage> UploadFileAsync(string endpoint, IFormFile file)
        {
            var client = GetClient();
            using var form = new MultipartFormDataContent();
            using var fileStream = file.OpenReadStream();
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            form.Add(fileContent, "file", file.FileName);
            return await client.PostAsync(endpoint, form);
        }

        public async Task<HttpResponseMessage> PutAsync(string endpoint, object data)
        {
            var client = GetClient();
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await client.PutAsync(endpoint, content);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            var client = GetClient();
            return await client.DeleteAsync(endpoint);
        }
    }
}