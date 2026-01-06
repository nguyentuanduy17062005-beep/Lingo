# 📚 LingoApp - Ứng Dụng Học Tiếng Anh

Ứng dụng học tiếng Anh với giao diện Windows Forms, hỗ trợ dịch thuật, làm bài test trắc nghiệm và học các thì tiếng Anh.

## ✨ Tính Năng

### 1. 🌍 Dịch Tiếng Anh (Google Translate API)
- Dịch từ Tiếng Việt sang Tiếng Anh và ngược lại
- Sử dụng Google Translate API miễn phí
- Giao diện đơn giản, dễ sử dụng
- Chức năng đổi ngôn ngữ nhanh

### 2. 📝 Bài Test Trắc Nghiệm
- **15 câu hỏi** mỗi bài test
- **3 mức độ khó:**
  - Dễ (Easy): 10 giây/câu
  - Trung bình (Normal): 15 giây/câu
  - Khó (Hard): 20 giây/câu
- Đếm ngược thời gian cho mỗi câu hỏi
- Tự động chuyển câu khi hết giờ
- Hiển thị kết quả chi tiết sau khi hoàn thành
- Tích điểm XP khi làm bài

### 3. 📚 Kho Các Thì Tiếng Anh (Local Storage)
- **12 thì tiếng Anh** đầy đủ:
  - Present Simple, Present Continuous, Present Perfect, Present Perfect Continuous
  - Past Simple, Past Continuous, Past Perfect, Past Perfect Continuous
  - Future Simple, Future Continuous, Future Perfect, Future Perfect Continuous
- Mỗi thì bao gồm:
  - Tên tiếng Anh và tiếng Việt
  - Mô tả chi tiết
  - Cấu trúc ngữ pháp
  - Cách sử dụng
  - Ví dụ minh họa
  - Dấu hiệu nhận biết
- Dữ liệu được lưu trữ cục bộ trong SQLite database

## 🚀 Cách Chạy Ứng Dụng

### Yêu Cầu
- .NET 8.0 SDK
- Windows OS

### Chạy Ứng Dụng
```powershell
dotnet run
```

### Build Ứng Dụng
```powershell
dotnet build
```

### Publish Ứng Dụng
```powershell
dotnet publish -c Release
```

## 📖 Hướng Dẫn Sử Dụng

### 1. Đăng Nhập/Đăng Ký
- Nhập **Username** và **Email**
- Nhấn **Đăng Ký** nếu là người dùng mới
- Nhấn **Đăng Nhập** để vào ứng dụng

### 2. Sử Dụng Tính Năng Dịch
- Chọn ngôn ngữ nguồn và ngôn ngữ đích
- Nhập văn bản cần dịch
- Nhấn nút **Dịch**
- Sử dụng nút **⇄** để đổi ngôn ngữ nhanh

### 3. Làm Bài Test
- Nhấn nút **Bài Test Trắc Nghiệm**
- Đọc câu hỏi và chọn đáp án A, B, C hoặc D
- Theo dõi thời gian đếm ngược
- Nhấn **Tiếp theo** để chuyển câu
- Nhấn **Nộp bài** ở câu cuối cùng
- Xem kết quả và điểm XP nhận được

### 4. Học Các Thì
- Nhấn nút **Kho Các Thì**
- Chọn thì muốn học từ danh sách bên trái
- Đọc thông tin chi tiết bên phải
- Học cấu trúc, cách dùng và ví dụ

## 🗄️ Cấu Trúc Database

Ứng dụng sử dụng SQLite với các bảng:
- **Users**: Thông tin người dùng
- **TensesData**: 12 thì tiếng Anh
- **QuizQuestions**: 15 câu hỏi trắc nghiệm
- **QuizResults**: Lịch sử làm bài test
- **Lessons, Achievements, Vocabulary**: Các tính năng khác

## 🎯 Tính Năng Đang Phát Triển

- ✅ Dịch tiếng Anh với Google Translate API
- ✅ Bài test trắc nghiệm 15 câu với timer
- ✅ Kho 12 thì tiếng Anh local
- 🔄 Học từ vựng
- 🔄 Luyện nghe
- 🔄 Luyện nói với AI

## 📦 Dependencies

- Microsoft.EntityFrameworkCore.Sqlite (8.0.11)
- .NET 8.0 Windows Forms

## 👨‍💻 Tác Giả

LingoApp - Ứng dụng học tiếng Anh hiện đại

## 📝 License

MIT License
