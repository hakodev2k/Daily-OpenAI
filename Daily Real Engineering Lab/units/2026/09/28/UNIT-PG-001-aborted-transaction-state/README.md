# UNIT-PG-001 — Transaction vẫn mở nhưng không còn dùng được

## Mục tiêu

Điều tra một batch import dùng PostgreSQL transaction: một thao tác tùy chọn gặp lỗi đã được catch, nhưng các lệnh SQL hợp lệ phía sau vẫn thất bại. Thu thập evidence, viết hypothesis, sửa transaction boundary và chứng minh batch giữ đúng tính atomic.

## Bối cảnh thực tế

Một worker nhập invoice chạy nhiều bước trong cùng transaction. Duplicate audit marker được xem là tình huống có thể bỏ qua. Log cho thấy exception duplicate đã được catch, nhưng bước cập nhật invoice phía sau lại thất bại và toàn bộ batch không commit.

## Bạn cần làm gì

1. Chạy starter và ghi lại SQLSTATE của từng lỗi.
2. Xác định transaction state sau lỗi đầu tiên.
3. Sửa code trong `src/LabApp/Program.cs` để duplicate audit marker có thể được bỏ qua mà transaction vẫn tiếp tục an toàn.
4. Không tách invoice update ra ngoài transaction chính.
5. Chạy `verify.ps1` và giải thích vì sao fix của bạn đúng.

## Yêu cầu môi trường

- Docker Desktop hoặc Docker Engine
- .NET 8 SDK
- PowerShell 7+ khuyến nghị

## Chạy nhanh

```powershell
docker compose up -d
dotnet restore src/LabApp/LabApp.csproj
dotnet run --project src/LabApp/LabApp.csproj
```

## Cách reproduce vấn đề

Starter tự seed một invoice và một audit marker trùng khóa. Chạy app một lần và quan sát batch không đạt kết quả mong muốn dù lỗi đầu tiên đã được catch.

## Những gì cần quan sát

- SQLSTATE của exception đầu tiên và exception ở lệnh kế tiếp.
- Invoice amount trước và sau batch.
- Audit marker có bị nhân đôi hay không.
- Transaction có thực sự commit hay không.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-1.md)
- [Hint 2](hints/hint-2.md)
- [Hint 3](hints/hint-3.md)

## Reference Solution

⚠️ Spoiler: [Reference solution](solution/README.md)

## Expected Results

Sau fix, verification phải xác nhận batch thành công, invoice amount được cập nhật đúng, audit marker vẫn chỉ có một bản ghi và transaction commit.

## Estimated Time

45–60 phút.
