using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.PronunciationAssessment;

namespace LingoAppNet8.Services
{
    public class PronunciationResult
    {
        public double AccuracyScore { get; set; }
        public double FluencyScore { get; set; }
        public double CompletenessScore { get; set; }
        public double PronunciationScore { get; set; }
        public string RecognizedText { get; set; } = "";
        public string ErrorMessage { get; set; } = "";
        public bool Success { get; set; }
    }

    public class SpeechRecognitionService
    {
        // Azure Speech Service credentials (FREE TIER: 5000 requests/month)
        // Bạn cần đăng ký tài khoản Azure miễn phí tại: https://azure.microsoft.com/free/
        // Sau đó tạo Speech Service và lấy key + region
        private const string AZURE_SPEECH_KEY = "YOUR_AZURE_SPEECH_KEY"; // Thay bằng key của bạn
        private const string AZURE_REGION = "southeastasia"; // Hoặc region gần bạn nhất

        public async Task<PronunciationResult> AssessPronunciationAsync(string expectedText, string audioFilePath)
        {
            // Sử dụng thuật toán local (Windows Speech Recognition)
            // Nếu muốn dùng Azure, cần cài đặt Azure Speech SDK phiên bản cũ hơn
            // hoặc sử dụng REST API của Azure Speech Service
            return await AssessPronunciationLocalAsync(expectedText, audioFilePath);
        }

        // Thuật toán local đơn giản (không cần Azure)
        private async Task<PronunciationResult> AssessPronunciationLocalAsync(string expectedText, string audioFilePath)
        {
            try
            {
                // Sử dụng Windows Speech Recognition (built-in)
                using var recognizer = new System.Speech.Recognition.SpeechRecognitionEngine();
                recognizer.SetInputToWaveFile(audioFilePath);
                recognizer.LoadGrammar(new System.Speech.Recognition.DictationGrammar());

                var result = recognizer.Recognize(TimeSpan.FromSeconds(10));

                if (result != null)
                {
                    string recognizedText = result.Text;
                    
                    // Thuật toán chấm điểm đơn giản dựa trên Levenshtein Distance
                    var scores = CalculatePronunciationScores(expectedText, recognizedText, result.Confidence);

                    return new PronunciationResult
                    {
                        Success = true,
                        AccuracyScore = scores.accuracy,
                        FluencyScore = scores.fluency,
                        CompletenessScore = scores.completeness,
                        PronunciationScore = scores.overall,
                        RecognizedText = recognizedText
                    };
                }
                else
                {
                    return new PronunciationResult
                    {
                        Success = false,
                        ErrorMessage = "Không nhận diện được giọng nói. Vui lòng thử lại."
                    };
                }
            }
            catch (Exception ex)
            {
                return new PronunciationResult
                {
                    Success = false,
                    ErrorMessage = $"Lỗi: {ex.Message}"
                };
            }
        }

        // Thuật toán tính điểm phát âm (ĐIỀU CHỈNH CHO NGƯỜI MỚI)
        private (double accuracy, double fluency, double completeness, double overall) CalculatePronunciationScores(
            string expected, string recognized, float confidence)
        {
            // 1. Chuẩn hóa text
            expected = expected.ToLower().Trim();
            recognized = recognized.ToLower().Trim();

            // 2. Tính Completeness Score (độ hoàn chỉnh) - NHẸ NHÀNG HƠN
            string[] expectedWords = expected.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string[] recognizedWords = recognized.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            // Cho điểm cơ bản nếu nói được ít nhất 1 từ
            double completeness = recognizedWords.Length > 0 ? 40 : 0; // Baseline 40 điểm
            completeness += Math.Min(60, (double)recognizedWords.Length / expectedWords.Length * 60);

            // 3. Tính Accuracy Score (độ chính xác) - DỄ TÍNH ĐIỂM HƠN
            int distance = LevenshteinDistance(expected, recognized);
            int maxLength = Math.Max(expected.Length, recognized.Length);
            
            // Tính similarity với công thức nhẹ nhàng hơn
            double similarity = maxLength > 0 ? (1.0 - (double)distance / maxLength) * 100 : 0;
            
            // Boost confidence multiplier (1.0 - 1.5 thay vì 0.0 - 1.0)
            double confidenceBoost = 0.5 + (confidence * 0.5); 
            
            // Cho điểm baseline 30 điểm nếu có nói
            double accuracy = recognizedWords.Length > 0 ? 30 : 0;
            accuracy += similarity * 0.7 * confidenceBoost; // Giảm độ khắt khe
            accuracy = Math.Min(100, accuracy);

            // 4. Tính Fluency Score (độ trôi chảy) - TĂNG ĐIỂM CHO TỪ GẦN ĐÚNG
            int matchedWords = 0;
            int partialMatches = 0;
            
            foreach (var word in recognizedWords)
            {
                if (expectedWords.Contains(word))
                {
                    matchedWords++;
                }
                else
                {
                    // Cho điểm nếu từ tương tự (chỉ khác vài ký tự)
                    foreach (var expectedWord in expectedWords)
                    {
                        int wordDistance = LevenshteinDistance(word, expectedWord);
                        if (wordDistance <= 2 && expectedWord.Length > 3) // Cho phép sai tối đa 2 ký tự
                        {
                            partialMatches++;
                            break;
                        }
                    }
                }
            }
            
            // Baseline 35 điểm nếu có nói
            double fluency = recognizedWords.Length > 0 ? 35 : 0;
            fluency += expectedWords.Length > 0 ? ((double)matchedWords / expectedWords.Length * 50) : 0;
            fluency += expectedWords.Length > 0 ? ((double)partialMatches / expectedWords.Length * 15) : 0; // Thưởng từ gần đúng
            fluency = Math.Min(100, fluency);

            // 5. Tính Overall Pronunciation Score - CÔNG THỨC NHẸ NHÀNG
            // Thêm 10 điểm bonus nếu cả 3 chỉ số đều > 30
            double bonus = (accuracy > 30 && fluency > 30 && completeness > 30) ? 10 : 0;
            
            // Trọng số ưu tiên Completeness (cố gắng nói hết) hơn Accuracy
            double overall = (accuracy * 0.30 + fluency * 0.35 + completeness * 0.35) + bonus;
            overall = Math.Min(100, overall);

            return (
                accuracy: Math.Round(accuracy, 2),
                fluency: Math.Round(fluency, 2),
                completeness: Math.Round(completeness, 2),
                overall: Math.Round(overall, 2)
            );
        }

        // Levenshtein Distance Algorithm - tính khoảng cách giữa 2 chuỗi
        private int LevenshteinDistance(string s1, string s2)
        {
            int[,] matrix = new int[s1.Length + 1, s2.Length + 1];

            for (int i = 0; i <= s1.Length; i++)
                matrix[i, 0] = i;
            
            for (int j = 0; j <= s2.Length; j++)
                matrix[0, j] = j;

            for (int i = 1; i <= s1.Length; i++)
            {
                for (int j = 1; j <= s2.Length; j++)
                {
                    int cost = s1[i - 1] == s2[j - 1] ? 0 : 1;
                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost
                    );
                }
            }

            return matrix[s1.Length, s2.Length];
        }
    }
}
