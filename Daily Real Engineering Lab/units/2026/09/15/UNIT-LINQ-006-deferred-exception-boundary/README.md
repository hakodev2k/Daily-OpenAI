# UNIT-LINQ-006 — Exception Boundary That Does Not Hold

## Mục tiêu

Điều tra một luồng import mà service được thiết kế để chuyển lỗi dữ liệu thành lỗi domain ổn định, nhưng caller vẫn nhận exception kỹ thuật thô ở một thời điểm bất ngờ.

## Bối cảnh thực tế

Một batch import đọc các amount dạng text. Contract của `ImportAmountService` yêu cầu dữ liệu sai phải được báo bằng `ImportDataException` để tầng gọi có thể map sang trạng thái nghiệp vụ phù hợp. Trong production, log cho thấy service đã trả control về caller, sau đó pipeline mới văng `FormatException` khi xử lý kết quả.

## Bạn cần làm gì

1. Chạy starter và xác nhận symptom.
2. Xác định chính xác thời điểm exception phát sinh so với lúc `GetAmounts` trả về.
3. Ghi ít nhất hai hypothesis trước khi sửa.
4. Sửa `starter/` để contract exception của service được giữ đúng.
5. Chạy `verify.ps1` và bảo đảm invalid input được chuyển thành `ImportDataException` có `FormatException` làm inner exception.

Không đổi dữ liệu mẫu và không sửa `verify.ps1` để làm test xanh.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7 hoặc Windows PowerShell

## Chạy nhanh

```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
dotnet run --project ./starter/DeferredBoundaryLab.csproj -- reproduce
```

Script thành công khi nó chứng minh starter làm `FormatException` thoát ra ngoài boundary mà service dự kiến kiểm soát.

## Những gì cần quan sát

- Dòng `Service returned. Enumeration starts now.` xuất hiện trước failure.
- Một số phần tử hợp lệ có thể đã được xử lý trước khi failure xuất hiện.
- Exception mà caller quan sát không phải loại exception được contract của service yêu cầu.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

[Spoiler — chỉ xem sau khi đã tự sửa](solution/README.md)

## Expected Results

**Before:** caller thấy raw `FormatException` sau khi service đã return.

**After:** invalid import data được translate thành `ImportDataException` tại boundary do service sở hữu; `verify.ps1` exit code 0.

## Estimated Time

30–45 phút.
