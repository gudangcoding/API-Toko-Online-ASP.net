# API Toko Online

API toko online yang dibangun dengan ASP.NET Core dan Entity Framework Core. API ini menyediakan endpoint untuk mengelola User, Kategori, Produk, Order, OrderItem, dan Riwayat aktivitas.

## Fitur Utama

- **User Management**: Registrasi, login, dan manajemen profil user
- **Category Management**: CRUD untuk kategori produk
- **Product Management**: CRUD untuk produk dengan stok management
- **Order Management**: Pembuatan order, tracking status, dan pembatalan
- **Order Items**: Detail item dalam setiap order
- **Activity History**: Pencatatan riwayat aktivitas user

## Teknologi yang Digunakan

- ASP.NET Core 9.0
- Entity Framework Core
- SQL Server (LocalDB)
- C# 12

## Struktur Database

### Entities

1. **User**
   - Id, Name, Email, Password, Phone, Address
   - CreatedAt, UpdatedAt, IsActive

2. **Category**
   - Id, Name, Description
   - CreatedAt, UpdatedAt, IsActive

3. **Product**
   - Id, Name, Description, Price, Stock, ImageUrl
   - CategoryId (Foreign Key)
   - CreatedAt, UpdatedAt, IsActive

4. **Order**
   - Id, OrderNumber, UserId, ShippingAddress, Phone
   - TotalAmount, Status, Notes
   - OrderDate, ShippedDate, DeliveredDate
   - CreatedAt, UpdatedAt

5. **OrderItem**
   - Id, OrderId, ProductId, Quantity
   - UnitPrice, TotalPrice, CreatedAt

6. **Riwayat**
   - Id, UserId, ActivityType, Description, Details
   - ActivityDate, IpAddress, UserAgent

## Setup dan Instalasi

### Prerequisites

- .NET 9.0 SDK
- SQL Server LocalDB (atau SQL Server Express)

### Langkah Instalasi

1. Clone repository ini
2. Navigasi ke direktori proyek
3. Restore dependencies:
   ```bash
   dotnet restore
   ```
4. Update connection string di `appsettings.json` jika diperlukan
5. Jalankan aplikasi:
   ```bash
   dotnet run
   ```

Database akan otomatis dibuat saat aplikasi pertama kali dijalankan.

## API Endpoints

### Users

- `POST /api/users/register` - Registrasi user baru
- `POST /api/users/login` - Login user
- `GET /api/users` - Mendapatkan semua user
- `GET /api/users/{id}` - Mendapatkan user by ID
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user (soft delete)

### Categories

- `GET /api/categories` - Mendapatkan semua kategori
- `GET /api/categories/{id}` - Mendapatkan kategori by ID
- `POST /api/categories` - Membuat kategori baru
- `PUT /api/categories/{id}` - Update kategori
- `DELETE /api/categories/{id}` - Delete kategori (soft delete)

### Products

- `GET /api/products` - Mendapatkan semua produk
- `GET /api/products/{id}` - Mendapatkan produk by ID
- `GET /api/products/category/{categoryId}` - Mendapatkan produk by kategori
- `POST /api/products` - Membuat produk baru
- `PUT /api/products/{id}` - Update produk
- `DELETE /api/products/{id}` - Delete produk (soft delete)

### Orders

- `GET /api/orders` - Mendapatkan semua order
- `GET /api/orders/{id}` - Mendapatkan order by ID
- `GET /api/orders/user/{userId}` - Mendapatkan order by user
- `POST /api/orders` - Membuat order baru
- `PUT /api/orders/{id}/status` - Update status order
- `DELETE /api/orders/{id}` - Cancel order

### Riwayats

- `GET /api/riwayats` - Mendapatkan semua riwayat
- `GET /api/riwayats/{id}` - Mendapatkan riwayat by ID
- `GET /api/riwayats/user/{userId}` - Mendapatkan riwayat by user
- `GET /api/riwayats/activity-types` - Mendapatkan tipe aktivitas
- `GET /api/riwayats/user/{userId}/activity/{activityType}` - Filter riwayat by aktivitas
- `GET /api/riwayats/user/{userId}/date-range` - Filter riwayat by rentang tanggal
- `POST /api/riwayats` - Membuat riwayat baru

## Contoh Request/Response

### Registrasi User

**Request:**
```json
POST /api/users/register
{
  "name": "John Doe",
  "email": "john@example.com",
  "password": "password123",
  "phone": "081234567890",
  "address": "Jl. Contoh No. 123"
}
```

**Response:**
```json
{
  "id": 1,
  "name": "John Doe",
  "email": "john@example.com",
  "phone": "081234567890",
  "address": "Jl. Contoh No. 123",
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": "2024-01-01T00:00:00Z",
  "isActive": true
}
```

### Membuat Order

**Request:**
```json
POST /api/orders
{
  "shippingAddress": "Jl. Pengiriman No. 456",
  "phone": "081234567890",
  "notes": "Tolong dibungkus rapi",
  "orderItems": [
    {
      "productId": 1,
      "quantity": 2
    },
    {
      "productId": 3,
      "quantity": 1
    }
  ]
}
```

**Response:**
```json
{
  "id": 1,
  "orderNumber": "ORD-20240101-ABC12345",
  "userId": 1,
  "userName": "John Doe",
  "shippingAddress": "Jl. Pengiriman No. 456",
  "phone": "081234567890",
  "totalAmount": 150000.00,
  "status": "Pending",
  "notes": "Tolong dibungkus rapi",
  "orderDate": "2024-01-01T00:00:00Z",
  "orderItems": [
    {
      "id": 1,
      "productId": 1,
      "productName": "Produk A",
      "quantity": 2,
      "unitPrice": 50000.00,
      "totalPrice": 100000.00
    }
  ]
}
```

## Status Order

- `Pending` - Order baru dibuat
- `Confirmed` - Order dikonfirmasi
- `Processing` - Order sedang diproses
- `Shipped` - Order sudah dikirim
- `Delivered` - Order sudah diterima
- `Cancelled` - Order dibatalkan

## Tipe Aktivitas

- `Login` - User login
- `Logout` - User logout
- `OrderCreated` - Order dibuat
- `OrderUpdated` - Order diupdate
- `OrderCancelled` - Order dibatalkan
- `ProfileUpdated` - Profil diupdate
- `PasswordChanged` - Password diubah

## Keamanan

- Password di-hash menggunakan SHA256
- Validasi input menggunakan Data Annotations
- Soft delete untuk data sensitif
- CORS policy untuk keamanan

## Pengembangan Selanjutnya

- Implementasi JWT Authentication
- Role-based Authorization
- Payment Gateway Integration
- Email Notifications
- File Upload untuk Product Images
- Pagination untuk list data
- Advanced Search dan Filtering
- API Rate Limiting

## Kontribusi

Silakan buat pull request untuk kontribusi atau laporkan bug melalui issues.

## Lisensi

Proyek ini menggunakan lisensi MIT.
