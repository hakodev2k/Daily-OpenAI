# UNIT-TEST-014 — Permit Approval: Green Tests, Missing Production State

## Mục tiêu
Điều tra vì sao cùng một lệnh phê duyệt permit được test báo thành công nhưng sau khi production đọc lại dữ liệu, trạng thái vẫn là `Pending`. Luyện cách kiểm chứng hành vi của test double so với persistence contract, không chỉ nhìn assertion xanh.

## Bối cảnh thực tế
Một hệ thống quản lý giấy phép xây dựng cho phép cán bộ duyệt hồ sơ. CI chạy unit tests xanh, API trả về `accepted=true`, nhưng bộ phận nghiệp vụ vẫn thấy hồ sơ chưa duyệt khi mở lại màn hình. Đội vận hành xác nhận không có lỗi kết nối database và không thấy exception.

## Bạn cần làm gì
1. Chạy hai chế độ mô phỏng cùng một nghiệp vụ; ghi lại sự khác biệt giữa kết quả trả về và trạng thái đọc lại.
2. Nêu ít nhất ba hypothesis và chỉ ra bằng chứng bác bỏ hoặc ủng hộ từng hypothesis.
3. Xác định contract cần giữ giữa business service, repository và test double.
4. Sửa mã trong `starter/` sao cho test theo persistence contract đi qua, đồng thời không phá các nhánh `not found` và `already approved`.
5. Giải thích vì sao test xanh ban đầu không đủ chứng minh tính đúng đắn.

## Yêu cầu môi trường
- .NET SDK 8.0+ và .NET 8 runtime; PowerShell 7+ hoặc Windows PowerShell.
- Không cần database, Docker, cloud hay NuGet ngoài .NET SDK.
- `global.json` khai báo SDK 8.0.100 với roll-forward phù hợp; project target `net8.0`.

## Chạy nhanh
Từ thư mục unit:

```powershell
./scripts/setup.ps1
./scripts/run.ps1
./scripts/reproduce.ps1
```

Trên Bash có thể dùng `dotnet run --project starter/PermitLab.csproj -- reproduce`.

## Cách reproduce vấn đề
Chạy `./scripts/reproduce.ps1` **trước khi sửa**. Script trả exit code 0 chỉ khi quan sát được sự bất nhất có chủ đích: một chế độ cho kết quả đúng sau khi reload, chế độ còn lại trả thành công nhưng dữ liệu đọc lại không đổi. Nếu đã sửa code, script reproduce có thể không còn pass; đó là hành vi đúng.

## Những gì cần quan sát
- Kết quả của lệnh `ApproveAsync` và trạng thái sau một lần đọc mới có giống nhau không?
- `Version` có tăng sau khi reload không?
- Số lần repository ghi nhận một thao tác lưu có tương ứng với kết quả nghiệp vụ không?
- Test double và mô hình gần production có thực sự áp dụng cùng một persistence contract không?

Có thêm dữ liệu triệu chứng tại [evidence/incident-log.md](evidence/incident-log.md).

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace/my-investigation.md](workspace/my-investigation.md).
3. Thử fix trên `starter/`.
4. Chạy `./scripts/verify.ps1` để kiểm tra **mã bạn đã sửa**, không phải reference solution.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
⚠️ Spoiler — chỉ mở [solution/README.md](solution/README.md) sau khi đã tự reproduce, phân tích và thử sửa.

## Expected Results
- **Before:** `reproduce.ps1` exit 0, `verify.ps1` exit khác 0.
- **After:** `verify.ps1` exit 0; kết quả trả về khớp trạng thái đã lưu, không có ghi thừa cho request không hợp lệ.
- Chi tiết: [before](expected-results/before.md) và [after](expected-results/after.md).

## Estimated Time
55 phút. Phần mở rộng Senior: [extensions/senior-challenge.md](extensions/senior-challenge.md).
