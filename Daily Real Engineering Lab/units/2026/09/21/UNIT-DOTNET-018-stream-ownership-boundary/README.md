# UNIT-DOTNET-018 — Stream Ownership Boundary

## Mục tiêu

Điều tra một lỗi lifecycle trong pipeline xuất file và xác định ownership contract phù hợp cho resource dùng chung giữa các tầng.

## Bối cảnh thực tế

Một service tạo CSV trong memory, gọi helper để bổ sung footer rồi chuyển stream sang bước upload. Luồng xử lý trông bình thường và helper hoàn thành thành công, nhưng bước upload phía sau không thể đọc dữ liệu. Refactor gần đây chỉ tách logic formatting thành helper riêng.

## Bạn cần làm gì

1. Chạy starter và reproduce triệu chứng.
2. Ghi hypothesis trong `workspace/my-investigation.md`.
3. Xác định lifecycle contract giữa caller và helper.
4. Sửa code trong `starter/` mà không bỏ việc cleanup resource ở đúng owner.
5. Chạy `verify.ps1`.
6. Sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

## Những gì cần quan sát

- Helper có hoàn thành trước khi lỗi xuất hiện không.
- Bước nào là consumer tiếp theo của cùng resource.
- Exception type và thời điểm resource không còn usable.
- Resource cần sống đến boundary nào của operation.

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

[Spoiler — chỉ xem sau khi đã tự thử](solution/README.md)

## Expected Results

Before: formatting hoàn thành nhưng downstream upload simulation không đọc được stream.

After: downstream đọc được payload đầy đủ và resource vẫn được cleanup sau khi toàn bộ operation kết thúc.

Nếu không reproduce được, chạy `dotnet --version` rồi `dotnet run --project starter -- --reproduce`.

## Estimated Time

Khoảng 35 phút.