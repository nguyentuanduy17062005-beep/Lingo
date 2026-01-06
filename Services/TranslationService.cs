using System.Net.Http;
using System.Text.Json;
using System.Web;

namespace LingoAppNet8.Services
{
    public class TranslationService
    {
        private static readonly HttpClient httpClient = new HttpClient();
        
        // Sử dụng Google Translate API (Free tier hoặc có thể dùng API key nếu có)
        public async Task<string> TranslateAsync(string text, string targetLanguage = "vi", string sourceLanguage = "en")
        {
            try
            {
                // Sử dụng Google Translate API miễn phí
                var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={sourceLanguage}&tl={targetLanguage}&dt=t&q={HttpUtility.UrlEncode(text)}";
                
                var response = await httpClient.GetStringAsync(url);
                
                // Parse JSON response
                using (JsonDocument doc = JsonDocument.Parse(response))
                {
                    var root = doc.RootElement;
                    if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
                    {
                        var firstElement = root[0];
                        if (firstElement.ValueKind == JsonValueKind.Array)
                        {
                            var translatedText = string.Empty;
                            foreach (var item in firstElement.EnumerateArray())
                            {
                                if (item.ValueKind == JsonValueKind.Array && item.GetArrayLength() > 0)
                                {
                                    translatedText += item[0].GetString();
                                }
                            }
                            return translatedText;
                        }
                    }
                }
                
                return text;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi dịch: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return text;
            }
        }

        // Dịch từ tiếng Việt sang tiếng Anh
        public async Task<string> TranslateToEnglishAsync(string text)
        {
            return await TranslateAsync(text, "en", "vi");
        }

        // Dịch từ tiếng Anh sang tiếng Việt
        public async Task<string> TranslateToVietnameseAsync(string text)
        {
            return await TranslateAsync(text, "vi", "en");
        }
    }
}
