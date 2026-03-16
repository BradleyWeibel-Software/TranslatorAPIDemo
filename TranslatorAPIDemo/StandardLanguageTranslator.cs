using System.Text;
using System.Text.Json;

namespace TranslatorAPIDemo
{
    public class StandardLanguageTranslator : ITranslator
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "bd1aa1b3-b134-4us5-94ab-32a22a3zzza353y1a:fx"; // DeepL API key
        private const string ApiUrl = "https://api-free.deepl.com/v1/translate";
        private static readonly IReadOnlyList<string> _supportedLanguages = Array.AsReadOnly(new[] { "bg", "de", "el", "en", "es", "fi", "fr", "it" });

        public IReadOnlyList<string> SupportedLanguages => _supportedLanguages;

        public StandardLanguageTranslator()
        {
            _httpClient = new HttpClient();
            _apiKey = _apiKey ?? Environment.GetEnvironmentVariable("DEEPL_API_KEY") ?? string.Empty;
        }

        public bool CanTranslate(string targetLanguage)
        {
            var lang = targetLanguage.ToUpper();
            return _supportedLanguages.Contains(lang.ToLower());
        }

        public async Task<TranslationResult> TranslateAsync(TranslationRequest request)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                return new TranslationResult
                {
                    Provider = "StandardTranslator (DeepL)",
                    TranslatedText = "Error: DeepL API key not configured"
                };
            }

            if (!CanTranslate(request.TargetLanguage))
            {
                return new TranslationResult
                {
                    Provider = "StandardTranslator (DeepL)",
                    TranslatedText = $"Error: Language {request.TargetLanguage} not supported"
                };
            }

            try
            {
                var payload = new
                {
                    text = new[] { request.Text },
                    source_lang = NormalizeLanguageCode(request.SourceLanguage),
                    target_lang = NormalizeLanguageCode(request.TargetLanguage)
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"
                );

                var request_msg = new HttpRequestMessage(HttpMethod.Post, ApiUrl)
                {
                    Content = content
                };
                request_msg.Headers.Add("Authorization", $"DeepL-Auth-Key {_apiKey}");

                var response = await _httpClient.SendAsync(request_msg);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);
                var translations = doc.RootElement.GetProperty("translations");
                var translatedText = translations[0].GetProperty("text").GetString();

                return new TranslationResult
                {
                    Provider = "StandardTranslator (DeepL)",
                    TranslatedText = translatedText ?? "No translation received"
                };
            }
            catch (HttpRequestException ex)
            {
                return new TranslationResult
                {
                    Provider = "StandardTranslator (DeepL)",
                    TranslatedText = $"Error: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new TranslationResult
                {
                    Provider = "StandardTranslator (DeepL)",
                    TranslatedText = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<bool> TestConnectionAsync()
        {
            if (string.IsNullOrEmpty(_apiKey))
                return false;

            try
            {
                var payload = new
                {
                    text = new[] { "test" },
                    source_lang = "en",
                    target_lang = "de"
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"
                );

                var request = new HttpRequestMessage(HttpMethod.Post, ApiUrl)
                {
                    Content = content
                };
                request.Headers.Add("Authorization", $"DeepL-Auth-Key {_apiKey}");

                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private string NormalizeLanguageCode(string code)
        {
            // DeepL uses uppercase codes like 'EN'
            return code.ToUpper();
        }
    }
}