# Concepts after lab

<code>JsonSerializerOptions</code> là cấu hình JSON contract đồng thời hỗ trợ metadata caching. Khi đã được sử dụng, instance chuyển sang read-only. Static helper dùng lại qua nhiều requests phải coi cấu hình như bất biến.

Harness xác nhận behavior: property camelCase, dữ liệu chính xác, format compact/pretty không lẫn nhau. Không cần test số lượng instance vì đó là chi tiết implementation.

Đánh đổi: tạo options mỗi request đơn giản nhưng tốn chi phí; cache hai profile bất biến phù hợp format hữu hạn; DI profile giúp quản lý cấu hình trong hệ thống lớn.

Tài liệu chính thức:
- [JsonSerializerOptions](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions)
- [Customize JSON properties](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/customize-properties)
