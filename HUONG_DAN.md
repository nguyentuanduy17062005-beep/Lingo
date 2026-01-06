# 📚 HƯỚNG DẪN SỬ DỤNG - LINGO APP

## 🎯 CÁC TÍNH NĂNG MỚI

### ✨ Cải Thiện Giao Diện
- **Giao diện hiện đại**: Màu sắc gradient, bo tròn góc, shadow effects
- **LoginForm**: Thiết kế đơn giản, dễ sử dụng với màu xanh gradient
- **MainForm**: Layout responsive với 6 button chức năng chính
- **Card-based UI**: Thông tin XP, Streak, Kỷ lục hiển thị rõ ràng

### 📋 Quản Lý Câu Hỏi (Mới!)
**Truy cập**: Click button "📋 QUẢN LÝ CÂU HỎI" trên màn hình chính

#### Chức năng:
1. **Thêm câu hỏi mới**
   - Click "➕ Thêm mới"
   - Nhập câu hỏi, 4 đáp án (A, B, C, D)
   - Chọn đáp án đúng
   - Chọn độ khó: Easy, Normal, Hard
   - Thiết lập thời gian (giây)
   - Chọn thì liên quan
   - Click "💾 Lưu"

2. **Sửa câu hỏi**
   - Click chọn câu hỏi trong danh sách
   - Click "✏️ Sửa" hoặc double-click vào câu hỏi
   - Chỉnh sửa thông tin
   - Click "💾 Lưu"

3. **Xóa câu hỏi**
   - Chọn câu hỏi cần xóa
   - Click "🗑️ Xóa"
   - Xác nhận xóa

4. **Tìm kiếm**
   - Gõ từ khóa vào ô "🔍 Tìm kiếm câu hỏi..."
   - Danh sách tự động lọc theo từ khóa

### 📂 Import Câu Hỏi Từ File

#### Định dạng file hỗ trợ:
- **.DOCX** (Microsoft Word)
- **.TXT** (Text file)

#### Format câu hỏi:
```
Q: Câu hỏi của bạn?
A: Đáp án A
B: Đáp án B
C: Đáp án C
D: Đáp án D
Answer: C
```

**Hoặc**
```
Question: Câu hỏi tiếng Anh?
A. First option
B. Second option
C. Third option
D. Fourth option
Correct: A
```

**Hoặc**
```
Câu: Câu hỏi tiếng Việt?
A) Lựa chọn 1
B) Lựa chọn 2
C) Lựa chọn 3
D) Lựa chọn 4
Đáp án: B
```

#### Cách import:
1. Chuẩn bị file DOCX hoặc TXT theo format trên
2. Mở "Quản Lý Câu Hỏi"
3. Click "📂 Import File (DOCX/TXT)"
4. Chọn file
5. Hệ thống tự động đọc và import câu hỏi
6. Thông báo số lượng câu hỏi đã import thành công

#### Lưu ý:
- Mỗi câu hỏi phải có đầy đủ 4 đáp án A, B, C, D
- Đáp án đúng phải ghi rõ: A, B, C hoặc D
- Có thể dùng nhiều cách đánh dấu: "Q:", "Question:", "Câu:"
- Đáp án có thể dùng ":", "." hoặc ")"
- File mẫu: **SampleQuestions.txt** trong thư mục project

---

## 🌟 CÁC TÍNH NĂNG CŨ (Đã Có)

### 1. 🎤 Luyện Phát Âm
- 34 câu mẫu với 3 mức độ (Easy, Medium, Hard)
- Chấm điểm phát âm tự động
- Dịch từng từ trong câu
- Nghe lại bản ghi âm

### 2. 📝 Bài Kiểm Tra
- 10 câu trắc nghiệm
- Đếm thời gian cho mỗi câu
- Hiển thị kết quả và điểm số

### 3. 📚 12 Thì Tiếng Anh
- Lý thuyết chi tiết
- Ví dụ minh họa
- Dấu hiệu nhận biết

### 4. 🌍 Dịch Tiếng Anh
- Dịch nhanh Anh - Việt

### 5. 📅 Điểm Danh
- Duy trì chuỗi ngày học liên tiếp
- Nhận XP mỗi ngày

---

## 🔧 HƯỚNG DẪN KỸ THUẬT

### Database: SQL Server
- **Server**: LAPTOP-7TOIFEJI\SQLEXPRESS
- **Database**: LingoDb
- **Connection**: Integrated Security (Windows Authentication)

### Packages sử dụng:
- Microsoft.EntityFrameworkCore.SqlServer 8.0.11
- DocumentFormat.OpenXml 3.2.0 (Đọc file DOCX)
- NAudio 2.2.1 (Thu âm)
- System.Speech 10.0.1 (Nhận dạng giọng nói)

### Build & Run:
```bash
cd LingoAppNet8
dotnet restore
dotnet build
dotnet run
```

---

## 📝 DEMO TÀI KHOẢN

**Username**: admin  
**Email**: admin@lingo.com

(Hoặc tạo tài khoản mới bằng nút ĐĂNG KÝ)

---

## 🎨 THIẾT KẾ MÀU SẮC

- **Gradient chính**: Blue (#4361EE) → Purple (#7367F0)
- **Success**: Green (#4CAF50)
- **Warning**: Orange (#FF9800)
- **Danger**: Red (#F44336)
- **Info**: Blue (#2196F3)
- **Purple**: #9C27B0

---

## 📧 HỖ TRỢ

Nếu có vấn đề, kiểm tra:
1. SQL Server đã chạy chưa?
2. Database LingoDb đã được tạo chưa?
3. File import có đúng format không?

Chúc bạn học tiếng Anh vui vẻ! 🚀
