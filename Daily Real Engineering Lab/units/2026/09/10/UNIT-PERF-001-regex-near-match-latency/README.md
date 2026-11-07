# UNIT-PERF-001 — Regex Near-Match Latency Spike

## Mục tiêu

Điều tra một đường validation có latency tăng mạnh theo dữ liệu đầu vào, thu thập evidence, xác định cơ chế gây ra hiện tượng và sửa mà không làm thay đổi contract hợp lệ/không hợp lệ.

## Bối cảnh thực tế

Một bulk-import worker xử lý mã định danh từ file CSV. Phần lớn record chạy nhanh, nhưng một số record gần giống dữ liệu hợp lệ khiến một worker giữ CPU lâu hơn đáng kể và đôi khi chạm validation timeout. Khi file có nhiều record kiểu này, throughput toàn batch giảm mạnh dù downstream database vẫn khỏe.

## Bạn cần làm gì

1. Chạy starter và reproduce triệu chứng.
2. Ghi evidence và ít nhất 3 hypotheses vào `workspace/my-investigation.md`.
3. Xác định tại sao chỉ một nhóm input cụ thể làm validation chậm bất thường.
4. Sửa code trong `starter/`.
5. Chạy `verify.ps1` để kiểm tra cả correctness lẫn behavior dưới adversarial input.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script phải quan sát được case validation bị timeout trên input near-match trong khi các case bình thường vẫn xử lý được.

## Những gì cần quan sát

- Input hợp lệ ngắn trả kết quả nhanh.
- Input không hợp lệ đơn giản cũng trả kết quả nhanh.
- Một input không hợp lệ nhưng có prefix dài giống dữ liệu hợp lệ tạo behavior khác rõ rệt.
- CPU work xảy ra trong validation path, không phải database hay network.
- Khi tăng độ dài input near-match, chi phí xử lý không tăng theo cách bạn kỳ vọng từ một validator đơn giản.

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

> Spoiler — chỉ xem sau khi đã reproduce và thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

Trước fix:
- normal inputs vẫn đúng contract;
- adversarial near-match input có thể tạo `REGEX_TIMEOUT=1`;
- worker có nguy cơ giảm throughput theo dữ liệu đầu vào.

Sau fix:
- valid inputs vẫn được accept;
- invalid inputs vẫn bị reject;
- adversarial input hoàn thành trong budget và không phát sinh timeout;
- `verify.ps1` in `VERIFY_PASS`.

Nếu không reproduce được timeout, tăng `AdversarialLength` trong `starter/Program.cs` từng bước nhỏ. Không tăng quá mức cần thiết vì mục tiêu là quan sát xu hướng, không phải treo process.

## Estimated Time

45–60 phút.
