using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace _5_7_5.Services
{
    public class AzureTranslationService
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string _subscriptionKey;
        private readonly string _endpoint;
        private readonly string _location;

        public AzureTranslationService(string subscriptionKey, string location)
        {
            _subscriptionKey = subscriptionKey;
            _endpoint = "https://api.cognitive.microsofttranslator.com/";
            _location = location;
        }

        public async Task<(string JapaneseText, string RomajiText)> TranslateToJapaneseWithRomajiAsync(string romanianText)
        {
            string japaneseText = await TranslateTextAsync(romanianText, "ro", "ja");

            string romajiText = await TransliterateTextAsync(japaneseText);

            return (japaneseText, romajiText);
        }

        public async Task<(string, string, string, string, string, string)> TranslateHaikuAsync(
            string line1, string line2, string line3)
        {
            var (japaneseLine1, romajiLine1) = await TranslateToJapaneseWithRomajiAsync(line1);
            var (japaneseLine2, romajiLine2) = await TranslateToJapaneseWithRomajiAsync(line2);
            var (japaneseLine3, romajiLine3) = await TranslateToJapaneseWithRomajiAsync(line3);

            return (japaneseLine1, japaneseLine2, japaneseLine3,
                    romajiLine1, romajiLine2, romajiLine3);
        }

        private async Task<string> TranslateTextAsync(string text, string fromLang, string toLang)
        {
            ///translation API route
            var route = $"/translate?api-version=3.0&from={fromLang}&to={toLang}";

            var requestBody = new[] { new { Text = text } };
            var requestBodyJson = JsonSerializer.Serialize(requestBody);

            ///request
            using var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_endpoint + route),
                Content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json"),
                Headers =
                {
                    { "Ocp-Apim-Subscription-Key", _subscriptionKey },
                    { "Ocp-Apim-Subscription-Region", _location }
                }
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(responseBody);

            return document.RootElement[0]
                .GetProperty("translations")[0]
                .GetProperty("text")
                .GetString() ?? string.Empty;
        }

        private async Task<string> TransliterateTextAsync(string japaneseText)
        {
            ///romaji
            var route = $"/transliterate?api-version=3.0&language=ja&fromScript=Jpan&toScript=Latn";

            ///request
            var requestBody = new[] { new { Text = japaneseText } };
            var requestBodyJson = JsonSerializer.Serialize(requestBody);

            using var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_endpoint + route),
                Content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json"),
                Headers =
                {
                    { "Ocp-Apim-Subscription-Key", _subscriptionKey },
                    { "Ocp-Apim-Subscription-Region", _location }
                }
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(responseBody);

            return document.RootElement[0]
                .GetProperty("text")
                .GetString() ?? string.Empty;
        }
    }
}