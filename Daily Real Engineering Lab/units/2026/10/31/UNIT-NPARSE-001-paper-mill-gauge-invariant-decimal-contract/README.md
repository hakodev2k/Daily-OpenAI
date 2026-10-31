# UNIT-NPARSE-001 — Paper Mill Gauge Import: Cross-Locale Decimal Contract

## Mục tiêu

Điều tra sự sai lệch kết quả đo độ dày cuộn giấy giữa các máy trạm ở nhiều quốc gia. Từ bằng chứng thực nghiệm, xác định ranh giới giữa dữ liệu trao đổi của thiết bị và cấu hình máy chạy, sửa và kiểm chứng hợp đồng dữ liệu.

## Bối cảnh thực tế

Máy gauge xuất file gồm các dòng `RollId;ThicknessMm`. Hợp đồng của thiết bị: số thập phân dùng dấu chấm, không dùng dấu phân tách hàng nghìn. Cùng một file được nhập ở US, Đức, Pháp và Việt Nam nhưng kết quả kiểm tra chất lượng không thống nhất; một số site báo lỗi, số khác cho giá trị bất thường. Dữ liệu trong lab là synthetic.

## Bạn cần làm gì

1. Chạy starter, ghi lại kết quả theo locale và raw input.
2. Đề xuất ít nhất hai hypothesis; phân biệt lỗi file, lỗi tính toán và lỗi môi trường.
3. Chỉ chỉnh `starter/GaugeImport.cs`, giữ nguyên hợp đồng thiết bị.
4. Verify dữ liệu hợp lệ và dữ liệu không hợp lệ trên **starter đã sửa**.

## Yêu cầu môi trường

Windows PowerShell 5.1+ hoặc PowerShell 7, .NET SDK 8+. Không cần Docker, cloud, database hoặc NuGet package ngoài.

## Chạy nhanh

```powershell
dotnet --version
./scripts/run.ps1
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề

`./scripts/reproduce.ps1` trả exit code 0 khi chứng minh được lỗi ở starter. Sau khi sửa, `./scripts/verify.ps1` phải trả exit code 0 khi mọi contract check đạt. Hai script kiểm tra **hai trạng thái khác nhau**.

## Những gì cần quan sát

- Cùng raw token `1.234`, `0.875`, `2.500` có được hiểu như nhau ở mọi site?
- Lỗi xảy ra ở bước đọc file, parse số hay tính toán?
- Token có dấu phẩy bị từ chối hay chấp nhận?
- Kết quả sau sửa phải độc lập locale và không làm lỏng wire contract.

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

**Spoiler:** [Reference Solution](solution/README.md) — chỉ xem sau khi đã tự điều tra.

## Expected Results

- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time

50 phút. Ghi reasoning vào [workspace](workspace/my-investigation.md).
