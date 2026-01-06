# 🎤 Tính Năng Luyện Phát Âm - Speaking Practice

## 📋 Tổng Quan

Tính năng **Luyện Phát Âm** cho phép người dùng:
- Đọc các câu tiếng Anh được hiển thị trên màn hình
- Ghi âm giọng nói của mình bằng micro
- Nhận **điểm đánh giá tự động** về phát âm

## 🔬 Thuật Toán Chấm Điểm

Hệ thống sử dụng 2 phương pháp:

### 1. **Azure Cognitive Services Speech SDK** (Phương pháp chính - Cần đăng ký)
- ✅ Chính xác nhất, chuyên nghiệp
- ✅ Hỗ trợ Pronunciation Assessment (đánh giá phát âm chi tiết)
- ✅ Miễn phí 5,000 requests/tháng
- 📊 Điểm chấm bao gồm:
  - **Accuracy Score** (Độ chính xác): So sánh từng âm với chuẩn
  - **Fluency Score** (Độ trôi chảy): Tốc độ và nhịp điệu
  - **Completeness Score** (Độ hoàn chỉnh): Số từ nói đầy đủ
  - **Pronunciation Score** (Điểm tổng): Tổng hợp 3 điểm trên

#### Cách Đăng Ký Azure Speech Service (MIỄN PHÍ):
1. Truy cập: https://azure.microsoft.com/free/
2. Tạo tài khoản Microsoft miễn phí (dùng email)
3. Vào Azure Portal: https://portal.azure.com
4. Tạo "Speech Service" resource
5. Lấy **API Key** và **Region** (ví dụ: southeastasia)
6. Mở file `Services/SpeechRecognitionService.cs`
7. Thay thế:
   ```csharp
   private const string AZURE_SPEECH_KEY = "YOUR_AZURE_SPEECH_KEY"; 
   // Thay bằng key của bạn
   
   private const string AZURE_REGION = "southeastasia"; 
   // Hoặc region gần bạn nhất
   ```

### 2. **Windows Speech Recognition** (Phương pháp dự phòng - Built-in)
- ✅ Không cần đăng ký, hoạt động offline
- ⚠️ Độ chính xác thấp hơn Azure
- 📊 Thuật toán tự phát triển:
  - **Levenshtein Distance**: Tính khoảng cách giữa văn bản mong đợi và văn bản nhận dạng được
  - **Word Matching**: Đếm số từ khớp
  - **Confidence Score**: Độ tin cậy của Windows Speech Recognizer

#### Công Thức Tính Điểm (Local Algorithm):

```
1. Completeness = (Số từ nói được / Số từ cần nói) × 100

2. Levenshtein Distance = Số thao tác cần thiết để biến chuỗi A thành chuỗi B

3. Accuracy = (1 - Distance/MaxLength) × 100 × Confidence

4. Fluency = (Số từ khớp / Tổng số từ) × 100

5. Overall Score = (Accuracy × 0.4) + (Fluency × 0.3) + (Completeness × 0.3)
```

## 🎯 Cách Sử Dụng

1. **Mở tính năng**: Nhấn nút "🎤 LUYỆN PHÁT ÂM" trên màn hình chính
2. **Đọc câu**: App hiển thị 1 câu tiếng Anh + bản dịch tiếng Việt
3. **Ghi âm**: **BẤM VÀ GIỮ** nút micro, đọc to câu đó, sau đó thả ra
4. **Nhận kết quả**: Hệ thống phân tích và hiển thị:
   - 📊 Điểm tổng (0-100)
   - 📈 Độ chính xác
   - 🎵 Độ trôi chảy
   - ✅ Độ hoàn chỉnh
   - 💬 Văn bản đã nhận dạng được

5. **Nghe lại**: Nhấn "🔊 NGHE LẠI" để nghe lại bản ghi âm
6. **Câu tiếp theo**: Nhấn "➡️ CÂU TIẾP THEO" để luyện câu mới

## 🎨 Màn Hình

- **Header**: Gradient tím đẹp mắt
- **Panel câu**: Hiển thị câu tiếng Anh, dịch, và category
- **Nút micro**: Đỏ khi sẵn sàng, tím khi đang ghi
- **Panel kết quả**: Hiển thị điểm với màu sắc:
  - 🟢 Xanh lá: 80-100 (Xuất sắc)
  - 🟠 Cam: 60-79 (Khá)
  - 🔴 Đỏ: 0-59 (Cần cải thiện)

## 💾 Dữ Liệu

### Database Tables:

**SpeakingSentence** - Câu luyện tập:
- EnglishText: Câu tiếng Anh
- VietnameseTranslation: Bản dịch
- Category: Danh mục (Daily, Business, Travel...)
- Level: Độ khó (Easy, Medium, Hard)

**SpeakingResult** - Kết quả luyện tập:
- UserId: ID người dùng
- SentenceId: ID câu
- AccuracyScore: Điểm chính xác
- FluencyScore: Điểm trôi chảy
- CompletenessScore: Điểm hoàn chỉnh
- PronunciationScore: Điểm tổng
- RecognizedText: Văn bản đã nhận dạng
- PracticeDate: Ngày luyện

## 🎁 Phần Thưởng XP

Người dùng nhận XP dựa trên điểm số:
```
XP Gained = Pronunciation Score / 5
```
Ví dụ: Điểm 80 → Nhận 16 XP

## 📦 Dependencies

- **NAudio** (2.2.1): Ghi âm từ micro
- **Microsoft.CognitiveServices.Speech** (1.47.0): Azure Speech SDK
- **System.Speech** (10.0.1): Windows Speech Recognition

## ⚠️ Lưu Ý

1. **Micro**: Cần có micro hoạt động tốt
2. **Internet**: Cần kết nối Internet nếu dùng Azure (phương pháp 1)
3. **Windows**: System.Speech chỉ hoạt động trên Windows
4. **Azure Key**: Nếu không config, tự động dùng phương pháp local (độ chính xác thấp hơn)

## 🚀 Tương Lai

Có thể mở rộng:
- 📊 Thống kê chi tiết từng âm sai
- 🎯 Luyện tập theo chủ đề
- 🏆 Thử thách phát âm hàng ngày
- 👥 So sánh với bạn bè
- 🎤 Nhận diện giọng nói native speaker

## 🛠️ Troubleshooting

**Lỗi: "Không nhận diện được giọng nói"**
- Kiểm tra micro đã cắm và được phép truy cập
- Nói to hơn và rõ ràng hơn
- Kiểm tra Windows Settings → Privacy → Microphone

**Lỗi: "Speech recognition failed"**
- Kiểm tra kết nối Internet
- Kiểm tra Azure Key đã đúng chưa
- Xem log file để biết chi tiết

**Điểm số thấp**
- Nói chậm và rõ ràng hơn
- Phát âm chuẩn theo mẫu Anh-Mỹ
- Luyện tập nhiều lần
