# UNIT-SEMAPHORELEASE-001 — Aquarium feeding gate: giới hạn concurrency bị vượt

## Mục tiêu
Điều tra sự cố giới hạn truy cập thiết bị trong một worker .NET, phân biệt timeout của yêu cầu với quyền sở hữu tài nguyên, và chứng minh bản sửa bằng regression test.

## Bối cảnh thực tế
Hệ thống điều khiển máng ăn của thủy cung chỉ cho phép một lệnh chạy tại một thời điểm. Khi một request bị hủy trong lúc chờ, log đôi khi cho thấy hai lệnh cùng đi vào vùng độc quyền.

## Bạn cần làm gì
Chạy bản starter, ghi timeline và hypothesis trước khi sửa. Không sửa test hoặc nới giới hạn thiết bị. Thay đổi `starter/Program.cs` để bảo toàn giới hạn dưới cancellation và exception.

## Yêu cầu môi trường
.NET SDK 8.0; PowerShell 7 hoặc Windows PowerShell 5.1. Không cần dịch vụ ngoài.

## Chạy nhanh
```powershell
./scripts/setup.ps1
./scripts/run.ps1
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` chạy chương trình ở chế độ `repro`; exit code khác 0 là lỗi dự kiến ở starter. Lặp lại nhiều lần nếu muốn, nhưng fixture sử dụng barrier để điều phối.

## Những gì cần quan sát
Ghi thứ tự acquire/cancel/enter/exit, số tác vụ đồng thời lớn nhất, và trạng thái permit khi các tác vụ kết thúc. So sánh với hợp đồng giới hạn một lệnh.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải pháp tham chiếu](solution/README.md)

## Expected Results
Trước sửa: fixture phát hiện đồng thời vượt giới hạn. Sau sửa: kiểm tra `verify.ps1` trả về exit code 0 và `maxConcurrent=1`.

## Estimated Time
40–55 phút.
