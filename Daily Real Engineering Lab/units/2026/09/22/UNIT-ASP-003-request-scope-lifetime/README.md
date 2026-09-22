# UNIT-ASP-003 — Request Scope Lifetime

## Mục tiêu
Điều tra một lỗi state bị lẫn giữa các request và sửa ownership/lifetime của các service mà không thay đổi business contract.

## Bối cảnh thực tế
Một Order API multi-tenant hoạt động đúng ở request đầu tiên nhưng request tiếp theo đôi khi ghi log tenant không khớp với tenant đang xử lý. Không có exception và dữ liệu đầu vào vẫn đúng.

## Bạn cần làm gì
1. Reproduce hiện tượng bằng starter.
2. Ghi symptoms, evidence và ít nhất 2 hypotheses vào `workspace/my-investigation.md`.
3. Xác định boundary nào sở hữu request state.
4. Sửa code trong `starter/`.
5. Chạy verify để chứng minh hai request độc lập không chia sẻ state ngoài ý muốn.

## Yêu cầu môi trường
- .NET SDK 10.0.x
- PowerShell 7+ hoặc Windows PowerShell

## Chạy nhanh
```powershell
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Script chạy hai request scope tuần tự với tenant khác nhau và yêu cầu starter biểu hiện mismatch xác định.

## Những gì cần quan sát
- Tenant đầu vào của từng request.
- Tenant mà service xử lý thực sự quan sát.
- Instance identity/lifetime của các dependency giữa hai request.
- Không suy luận từ exception vì scenario này có thể hoàn thành mà không throw.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[Spoiler — chỉ mở sau khi tự thử](solution/README.md)

## Expected Results
Before: request thứ hai có thể quan sát tenant state thuộc request trước.

After: mỗi request quan sát đúng tenant của chính scope đó và verification trả exit code 0.

Nếu không reproduce được, chạy `dotnet --version`, sau đó `dotnet run --project starter -- --mode reproduce` và kiểm tra output `EXPECTED_MISMATCH`.

## Estimated Time
35 phút.