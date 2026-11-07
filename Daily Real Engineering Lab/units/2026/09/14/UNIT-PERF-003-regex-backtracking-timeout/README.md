# UNIT-PERF-003 — Regex Validation Latency Spike

## Mục tiêu

Điều tra một validator hoạt động bình thường với input phổ biến nhưng có thể trở nên rất chậm với một số input gần hợp lệ. Mục tiêu là xác định cơ chế gây degradation, sửa implementation và verify rằng behavior hợp lệ/không hợp lệ vẫn đúng.

## Bối cảnh thực tế

Một CMS backend validate slug trước khi publish content. Với slug thông thường, validation gần như tức thì. Tuy nhiên một số request chứa chuỗi dài gần hợp lệ làm request chậm bất thường và đôi khi vượt timeout của validator, trong khi CPU tăng trên instance xử lý request.

## Bạn cần làm gì

- Reproduce hiện tượng bằng starter.
- Ghi lại evidence và hypothesis trước khi sửa.
- Xác định vì sao thời gian xử lý phụ thuộc mạnh vào shape của input.
- Sửa code trong `starter/` để validation có thời gian xử lý ổn định hơn.
- Giữ nguyên functional contract: slug hợp lệ phải pass, slug không hợp lệ phải fail.
- Chạy `verify.ps1` để xác nhận.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell

## Chạy nhanh

```powershell
cd "Daily Real Engineering Lab/units/2026/09/14/UNIT-PERF-003-regex-backtracking-timeout"
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Starter thử nhiều input có độ dài tăng dần và giới hạn thời gian cho từng phép validation để tránh một input làm lab bị treo vô hạn.

## Những gì cần quan sát

- Input hợp lệ ngắn được xử lý nhanh và trả về `true`.
- Input không hợp lệ đơn giản trả về `false`.
- Một nhóm input gần hợp lệ có thể chạm timeout dù chỉ khác input bình thường ở phần cuối chuỗi.
- Failure phụ thuộc vào cấu trúc input hơn là chỉ độ dài tuyệt đối.

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

> Spoiler: chỉ xem sau khi đã reproduce và tự thử sửa.

[Reference Solution](solution/README.md)

## Expected Results

### Before

- `reproduce.ps1` tìm được ít nhất một near-match input làm validator timeout.
- Functional checks cơ bản vẫn có vẻ đúng.

### After

- `verify.ps1` không ghi nhận timeout với bộ input kiểm tra.
- Input hợp lệ vẫn trả về `true`.
- Input không hợp lệ vẫn trả về `false`.

Nếu máy của bạn không reproduce được timeout, tăng `MaxLength` trong starter thêm từng bước nhỏ, nhưng giữ timeout để tránh run không kiểm soát.

## Estimated Time

30–45 phút.
