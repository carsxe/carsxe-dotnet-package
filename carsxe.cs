using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace carsxe
{
    /// <summary>
    /// CarsXE API client for .NET (method parameter shapes match the Swift example:
    /// GET endpoints take IDictionary&lt;string,string&gt; and POST endpoints take a single imageUrl string)
    /// </summary>
    public class CarsXE : IAsyncDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly bool _disposeHttpClient;

        /// <summary>
        /// Create a new CarsXE client.
        /// </summary>
        /// <param name="apiKey">Your CarsXE API key.</param>
        /// <param name="httpClient">Optional HttpClient instance to use. If null, a new HttpClient will be created and disposed by the client.</param>
        /// <param name="apiBaseUrl">Optional base URL for the API (defaults to https://api.carsxe.com).</param>
        public CarsXE(string apiKey, HttpClient? httpClient = null, string? apiBaseUrl = null)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _baseUrl = apiBaseUrl?.TrimEnd('/') ?? "https://api.carsxe.com";
            if (httpClient is null)
            {
                _httpClient = new HttpClient();
                _disposeHttpClient = true;
            }
            else
            {
                _httpClient = httpClient;
                _disposeHttpClient = false;
            }
        }

        public string GetApiKey() => _apiKey;
        public string GetBaseUrl() => _baseUrl;

        private static string BuildQueryString(IDictionary<string, string> parameters)
        {
            var parts = parameters
                .Where(kv => kv.Value != null)
                .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}");
            return string.Join("&", parts);
        }

        private async Task<JsonDocument> GetAsync(string endpoint, IDictionary<string, string> queryParams)
        {
            // add auth + source
            var dict = new Dictionary<string, string>(queryParams ?? new Dictionary<string, string>(), StringComparer.OrdinalIgnoreCase)
            {
                ["key"] = _apiKey,
                ["source"] = "c#"
            };

            var query = BuildQueryString(dict);
            var url = $"{_baseUrl.TrimEnd('/')}/{endpoint}";
            if (!string.IsNullOrEmpty(query))
            {
                url = $"{url}?{query}";
            }

            using var res = await _httpClient.GetAsync(url).ConfigureAwait(false);
            var content = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!res.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Request to {url} failed with {(int)res.StatusCode}: {res.ReasonPhrase}. Body: {content}");
            }
            return JsonDocument.Parse(content);
        }

        private async Task<JsonDocument> PostJsonAsync(string endpoint, object body, IDictionary<string, string>? queryParams = null)
        {
            var dict = new Dictionary<string, string>(queryParams ?? new Dictionary<string, string>(), StringComparer.OrdinalIgnoreCase);
            dict["key"] = _apiKey;
            dict["source"] = "c#";

            var query = BuildQueryString(dict);
            var url = $"{_baseUrl.TrimEnd('/')}/{endpoint}";
            if (!string.IsNullOrEmpty(query))
            {
                url = $"{url}?{query}";
            }

            var json = JsonSerializer.Serialize(body);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var res = await _httpClient.PostAsync(url, content).ConfigureAwait(false);
            var resp = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!res.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"POST to {url} failed with {(int)res.StatusCode}: {res.ReasonPhrase}. Body: {resp}");
            }
            return JsonDocument.Parse(resp);
        }

        private static void Require(params (bool condition, string name)[] checks)
        {
            foreach (var (condition, name) in checks)
            {
                if (!condition)
                    throw new ArgumentException($"Missing required parameter: {name}");
            }
        }

        // --- Public API (signatures match the Swift example parameter shapes) ---

        // specs: GET /specs
        // Required: vin
        public async Task<JsonDocument> Specs(IDictionary<string, string> parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            Require((parameters.ContainsKey("vin") && !string.IsNullOrWhiteSpace(parameters["vin"]), "vin"));
            return await GetAsync("specs", parameters).ConfigureAwait(false);
        }

        // marketValue: GET /v2/marketvalue
        // Required: vin
        public async Task<JsonDocument> MarketValue(IDictionary<string, string> parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            Require((parameters.ContainsKey("vin") && !string.IsNullOrWhiteSpace(parameters["vin"]), "vin"));
            return await GetAsync("v2/marketvalue", parameters).ConfigureAwait(false);
        }

        // history: GET /history
        // Required: vin
        public async Task<JsonDocument> History(IDictionary<string, string> parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            Require((parameters.ContainsKey("vin") && !string.IsNullOrWhiteSpace(parameters["vin"]), "vin"));
            return await GetAsync("history", parameters).ConfigureAwait(false);
        }

        // recalls: GET /v1/recalls
        // Required: vin
        public async Task<JsonDocument> Recalls(IDictionary<string, string> parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            Require((parameters.ContainsKey("vin") && !string.IsNullOrWhiteSpace(parameters["vin"]), "vin"));
            return await GetAsync("v1/recalls", parameters).ConfigureAwait(false);
        }

        // internationalVinDecoder: GET /v1/international-vin-decoder
        // Required: vin
        public async Task<JsonDocument> InternationalVinDecoder(IDictionary<string, string> parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            Require((parameters.ContainsKey("vin") && !string.IsNullOrWhiteSpace(parameters["vin"]), "vin"));
            return await GetAsync("v1/international-vin-decoder", parameters).ConfigureAwait(false);
        }

        // platedecoder: GET /v2/platedecoder
        // Required: plate, country (state/district optional)
        public async Task<JsonDocument> PlateDecoder(IDictionary<string, string> parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            Require((parameters.ContainsKey("plate") && !string.IsNullOrWhiteSpace(parameters["plate"]), "plate"));
            // default country to US if not supplied
            if (!parameters.ContainsKey("country") || string.IsNullOrWhiteSpace(parameters["country"]))
            {
                parameters["country"] = "US";
            }

            var country = parameters["country"]?.Trim().ToLowerInvariant() ?? "us";
            if (country == "pk" || country == "pakistan")
            {
                Require((parameters.ContainsKey("state") && !string.IsNullOrWhiteSpace(parameters["state"]), "state"),
                        (parameters.ContainsKey("district") && !string.IsNullOrWhiteSpace(parameters["district"]), "district"));
            }
            else
            {
                Require((parameters.ContainsKey("state") && !string.IsNullOrWhiteSpace(parameters["state"]), "state"));
            }

            return await GetAsync("v2/platedecoder", parameters).ConfigureAwait(false);
        }

        // images: GET /images
        // Required: make, model
        public async Task<JsonDocument> Images(IDictionary<string, string> parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            Require((parameters.ContainsKey("make") && !string.IsNullOrWhiteSpace(parameters["make"]), "make"),
                    (parameters.ContainsKey("model") && !string.IsNullOrWhiteSpace(parameters["model"]), "model"));
            return await GetAsync("images", parameters).ConfigureAwait(false);
        }

        // obdcodesdecoder: GET /obdcodesdecoder
        // Required: code
        public async Task<JsonDocument> ObdCodesDecoder(IDictionary<string, string> parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            Require((parameters.ContainsKey("code") && !string.IsNullOrWhiteSpace(parameters["code"]), "code"));
            return await GetAsync("obdcodesdecoder", parameters).ConfigureAwait(false);
        }

        // plateImageRecognition: POST /platerecognition
        // Required: imageUrl (string)
        public async Task<JsonDocument> PlateImageRecognition(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new ArgumentException("Missing required parameter: imageUrl");

            var body = new { image = imageUrl };
            return await PostJsonAsync("platerecognition", body).ConfigureAwait(false);
        }

        // vinOcr: POST /v1/vinocr
        // Required: imageUrl (string)
        public async Task<JsonDocument> VinOcr(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new ArgumentException("Missing required parameter: imageUrl");

            var body = new { image = imageUrl };
            return await PostJsonAsync("v1/vinocr", body).ConfigureAwait(false);
        }

        // yearMakeModel: GET /v1/ymm
        // Required: year, make, model
        public async Task<JsonDocument> YearMakeModel(IDictionary<string, string> parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            Require((parameters.ContainsKey("year") && !string.IsNullOrWhiteSpace(parameters["year"]), "year"),
                    (parameters.ContainsKey("make") && !string.IsNullOrWhiteSpace(parameters["make"]), "make"),
                    (parameters.ContainsKey("model") && !string.IsNullOrWhiteSpace(parameters["model"]), "model"));
            return await GetAsync("v1/ymm", parameters).ConfigureAwait(false);
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposeHttpClient)
            {
                _httpClient.Dispose();
            }
            GC.SuppressFinalize(this);
            await Task.CompletedTask;
        }
    }
}