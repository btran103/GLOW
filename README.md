# 🌈 GLOW — Personal Life Management & Reflection

> *"Ghi lại cuộc sống, hiểu chính mình và nhìn thấy hành trình trưởng thành qua từng ngày."* ✨

**GLOW** là một ứng dụng quản lý cuộc sống cá nhân mang phong cách Fresh – Colorful – Playful được thiết kế đặc biệt dành cho thế hệ Gen Z. Không chỉ khô khan với những con số hay to-do list, GLOW giống như một "người bạn số" giúp bạn chăm sóc sức khỏe tinh thần, lưu giữ mọi khoảnh khắc thanh xuân và thấu hiểu bản thân thông qua trí tuệ nhân tạo (AI).

---

## 🚀 Các Tính Năng Nổi Bật

### 🔐 Tài khoản & Quyền riêng tư
- **Xác thực an toàn:** Đăng ký và đăng nhập với hệ thống gửi mã OTP thật trực tiếp qua Email (Gmail SMTP).
- **Hồ sơ cá nhân:** Cập nhật thông tin, thay đổi ảnh đại diện (Avatar upload).
- **Quyền riêng tư tối đa:** Tuỳ chọn tắt/mở theo dõi vị trí, phân tích AI, và bảo vệ ảnh Camera cục bộ.

### 🎯 Quản lý Năng suất
- **📚 Học tập (Pomodoro):** Đồng hồ đếm ngược tập trung học tập, có hiệu ứng âm thanh lofi thư giãn.
- **💼 Công việc (Kanban):** Quản lý đầu việc kéo-thả trực quan (To Do, Doing, Done).
- **💰 Chi tiêu:** Quản lý thu chi hàng ngày với biểu đồ trực quan thống kê tài chính.

### 🌿 Sức Khỏe & Thể Chất
- **❤️ Sức khỏe:** Theo dõi lượng nước uống, số giờ ngủ và thống kê chu kỳ sinh hoạt.
- **🍎 Ăn uống:** Quản lý lượng Calories nạp vào và đề xuất món ăn mỗi ngày.

### 🎨 Cảm Xúc & Kỷ Niệm
- **😊 Cảm xúc & Nhật ký AI:** Ghi lại tâm trạng và để AI tự động tổng hợp thành những trang nhật ký đong đầy cảm xúc.
- **📸 Kỷ niệm:** Chụp ảnh trực tiếp từ trình duyệt, áp dụng các filter màu sắc mang phong cách film/vintage.
- **📍 Bản đồ (Map):** Lưu lại những nơi bạn đã đi qua trên bản đồ thế giới (Tích hợp Leaflet.js).

### ✨ Góc Chữa Lành & Giải Trí
- **🔮 Tarot:** Rút bài Tarot 3D mỗi ngày để nhận thông điệp vũ trụ.
- **🌱 Phát triển & Hiểu mình:** Trắc nghiệm tâm lý, định hướng phát triển bản thân.
- **⏳ Capsule:** Gửi thư cho chính mình trong tương lai.

---

## 🛠️ Công Nghệ Sử Dụng

- **Backend:** C# / ASP.NET Core MVC (.NET 10)
- **Cơ sở dữ liệu:** SQLite (Zero-config, tự động khởi tạo)
- **Frontend:** HTML5, CSS3 (Custom Variables), Vanilla JS
- **Thư viện tích hợp:** Chart.js (Biểu đồ), Leaflet.js (Bản đồ)
- **UI/UX Design:** Bảng màu Pastel trẻ trung (Mint, Yellow, Lime, Pink, Lavender).

---

## ⚙️ Hướng Dẫn Cài Đặt & Chạy Dự Án

### 1. Yêu cầu hệ thống
- Cài đặt [.NET 10.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- Visual Studio 2022 (phiên bản tím) hoặc VS Code.

### 2. Cấu hình Email (Để gửi OTP thật)
Mở file `src/GLOW.Web/appsettings.Development.json` và điền thông tin Gmail của bạn:
```json
"Smtp": {
  "Username": "email_cua_ban@gmail.com",
  "Password": "mat_khau_ung_dung_gmail"
}
```
*(Nếu chưa cấu hình, ứng dụng vẫn sẽ hiển thị mã OTP trên màn hình web để bạn test dễ dàng).*

### 3. Khởi chạy
Mở Terminal/PowerShell tại thư mục dự án và chạy:
```bash
cd src/GLOW.Web
dotnet run
```
Truy cập vào trình duyệt: `http://localhost:5000` (hoặc cổng hiển thị trên Terminal).

---

🌈 **GLOW** — *Keep glowing and growing!*
