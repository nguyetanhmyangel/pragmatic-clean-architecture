* Trong appsettings.Development.json: "ConnectionStrings": {
  "DefaultConnection": "Host=bookify-db;Port=5432;Database=bookify;Username=postgres;Password=Admin@123"
  }

* Và trong appsettings.Docker.json (chạy trong container,Host=bookify-db vì đây là tên service trong docker-compose.yml):
"ConnectionStrings": {
"DefaultConnection": "Host=bookify-db;Port=5432;Database=bookify;Username=postgres;Password=Admin@123"
}