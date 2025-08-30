# API Toko Online - ASP.NET Core dengan JWT Authentication

API toko online yang dibangun dengan ASP.NET Core dan Entity Framework Core. API ini menyediakan endpoint untuk mengelola User, Kategori, Produk, Order, OrderItem, dan Riwayat aktivitas dengan sistem autentikasi JWT yang aman. Proyek ini dibuat untuk tujuan pembelajaran di kursus ASP.NET membuat backend dengan c# di lembaga kursus LKP Naura

## 🚀 Fitur Utama

- **User Management**: Registrasi, login, dan manajemen user dengan JWT authentication
- **Category Management**: CRUD operasi untuk kategori produk
- **Product Management**: CRUD operasi untuk produk dengan manajemen stok
- **Order Management**: Pembuatan order, tracking status, dan manajemen stok otomatis
- **Activity History**: Pencatatan dan tracking aktivitas user
- **JWT Authentication**: Sistem autentikasi yang aman dengan token
- **Swagger Documentation**: Dokumentasi API yang lengkap dan interaktif
- **Soft Delete**: Penghapusan data yang aman tanpa kehilangan data

## 🛠️ Teknologi yang Digunakan

- **.NET 9.0** - Framework terbaru untuk pengembangan web
- **ASP.NET Core** - Framework untuk building web APIs
- **Entity Framework Core** - ORM untuk database operations
- **SQL Server LocalDB** - Database lokal untuk development
- **JWT (JSON Web Token)** - Sistem autentikasi stateless
- **Swagger/OpenAPI** - Dokumentasi dan testing API
- **C#** - Bahasa pemrograman utama

## 📋 Prerequisites

Sebelum memulai, pastikan Anda memiliki:

1. **Visual Studio 2022** atau **Visual Studio Code** dengan C# extension
2. **.NET 9.0 SDK** - Download dari [dotnet.microsoft.com](https://dotnet.microsoft.com/download)
3. **SQL Server LocalDB** (biasanya sudah terinstall dengan Visual Studio)
4. **Git** (opsional, untuk version control)

## 🏗️ Langkah-langkah Pembangunan

### Langkah 1: Membuat Proyek Baru

```bash
# Buat direktori baru
mkdir MyApiApp
cd MyApiApp

# Buat proyek ASP.NET Core Web API
dotnet new webapi

# Restore dependencies
dotnet restore
```

### Langkah 2: Menambahkan Dependencies

Edit file `MyApiApp.csproj` dan tambahkan package berikut:

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="9.0.5" />
  <PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.5" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.5" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.5" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.5" />
  <PackageReference Include="Swashbuckle.AspNetCore" Version="7.0.0" />
  <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.5" />
  <PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.3.0" />
</ItemGroup>
```

### Langkah 3: Membuat Model Entities

Buat folder `Models` dan buat file-file berikut:

#### 3.1 User Model (`Models/User.cs`)
#### 3.2 Category Model (`Models/Category.cs`)
#### 3.3 Product Model (`Models/Product.cs`)
#### 3.4 Order Model (`Models/Order.cs`)
#### 3.5 OrderItem Model (`Models/OrderItem.cs`)
#### 3.6 Riwayat Model (`Models/Riwayat.cs`)

### Langkah 4: Membuat Database Context

Buat folder `Data` dan buat `ApplicationDbContext.cs`

### Langkah 5: Membuat DTOs

Buat folder `DTOs` dan buat file-file DTO untuk request/response

### Langkah 6: Membuat Services

Buat folder `Services` dan implementasikan business logic

### Langkah 7: Membuat Custom Authorization

Buat folder `Attributes` dan buat `AuthorizeAttribute.cs`

### Langkah 8: Membuat Controllers

Buat folder `Controllers` dan implementasikan API endpoints

### Langkah 9: Konfigurasi Program.cs

Update Program.cs dengan JWT authentication dan Swagger

### Langkah 10: Konfigurasi Database

Update `appsettings.json` dengan connection string dan JWT settings

## 🔐 Cara Menggunakan JWT Authentication

### 1. Register User Baru
### 2. Login dan Dapatkan Token
### 3. Gunakan Token untuk Akses Endpoint Protected

## 📚 API Endpoints

### Public Endpoints (Tidak Perlu Token)
- User registration dan login
- Get categories dan products
- Get user information

### Protected Endpoints (Perlu Token)
- User management (update, delete)
- Order management
- Activity history

## 🧪 Testing dengan Swagger

1. Buka browser dan kunjungi: `http://localhost:5158`
2. Swagger UI akan menampilkan semua endpoint yang tersedia
3. Test endpoint secara interaktif

## 🔧 Troubleshooting

### Error: "Could not copy file because it is being used by another process"
- Stop aplikasi yang sedang berjalan dengan `Ctrl+C`
- Atau gunakan `taskkill /F /IM MyApiApp.exe`

### Error: "Email already registered"
- Gunakan email yang berbeda untuk registrasi

### Error: "Invalid token"
- Pastikan token JWT valid dan belum expired
- Pastikan format header: `Authorization: Bearer {token}`

## 📁 Struktur Folder

```
MyApiApp/
├── Controllers/           # API Controllers
├── Data/                 # Database Context
├── DTOs/                 # Data Transfer Objects
├── Models/               # Entity Models
├── Services/             # Business Logic Services
├── Attributes/           # Custom Attributes
├── Properties/           # Launch Settings
├── appsettings.json      # Configuration
├── Program.cs            # Application Entry Point
├── MyApiApp.csproj      # Project File
└── README.md            # This File
```

## 🚀 Deployment

### Development
```bash
dotnet run
```

### Production
```bash
dotnet publish -c Release
dotnet bin/Release/net9.0/publish/MyApiApp.dll
```

## 🔒 Security Considerations

1. **JWT Secret Key**: Gunakan secret key yang kuat dan aman
2. **Password Hashing**: Password di-hash menggunakan SHA256
3. **Token Expiration**: Token JWT memiliki masa berlaku (default: 60 menit)
4. **Input Validation**: Semua input divalidasi menggunakan Data Annotations
5. **SQL Injection Protection**: Menggunakan Entity Framework Core yang aman

## 🔮 Future Enhancements

1. **Role-based Authorization**: Admin, User, Moderator roles
2. **Refresh Token**: Implementasi refresh token untuk security yang lebih baik
3. **Email Verification**: Verifikasi email saat registrasi
4. **Password Reset**: Fitur reset password
5. **File Upload**: Upload gambar produk
6. **Payment Integration**: Integrasi payment gateway
7. **Real-time Notifications**: WebSocket untuk notifikasi real-time
8. **Caching**: Redis caching untuk performance
9. **Logging**: Structured logging dengan Serilog
10. **Unit Testing**: Unit tests untuk semua services dan controllers

## 📞 Support

Jika ada pertanyaan atau masalah, silakan buat issue di repository atau hubungi developer.

---

**Happy Coding! 🎉**
