# Reference Solution — SPOILER

## Root cause
`new UTF8Encoding(encoderShouldEmitUTF8Identifier: true)` khiến `StreamWriter` ghi UTF-8 BOM `EF BB BF` trước `HDR`. Editor thường không hiển thị BOM; gateway đối chiếu byte offset 0 nên từ chối. Không phải lỗi Unicode của tên thuốc.

## Patch tối thiểu
Trong `starter/Program.cs`, thay:
```csharp
new UTF8Encoding(encoderShouldEmitUTF8Identifier: true)
```
bằng:
```csharp
new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)
```

## Verification
Chạy `./verify.ps1` hoặc `dotnet run --project starter/PharmacyEdi.csproj -- verify`. Byte đầu phải là `4844527C3230323631313134`, `HEADER_AT_OFFSET_ZERO=True`, `EXACT_UTF8_PAYLOAD=True`. Kết quả runtime chưa được chạy trong phiên tạo lab; cần thực thi tại máy học viên.

## Wrong fixes
- `Trim()` trên text: có thể thay đổi payload và không đảm bảo loại BOM trên byte stream.
- Đổi sang ASCII: làm hỏng `Thuốc cảm`.
- Bỏ kiểm tra offset tại gateway: che giấu vi phạm contract, tạo rủi ro với parser cố định.
- Xóa ba byte đầu mọi file: phá file vốn không có BOM.

## Trade-offs
Chọn encoding tại ranh giới wire contract; regression test nên kiểm tra bytes và ký tự Unicode. Với đối tác cho phép BOM, document contract riêng thay vì mặc định mọi exporter giống nhau.
