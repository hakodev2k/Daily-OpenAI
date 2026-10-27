# Reference Solution — SPOILER

Chỉ đọc sau khi đã reproduce, ghi hypothesis và tự sửa `starter/ConversionRunner.cs`.

## 1. Symptoms
Batch phụ đề timeout khi diagnostic nhiều. Converter được start, CPU thấp, chưa có caption result.

## 2. Evidence
Worker ghi 1,048,576 ký tự vào stderr trước khi ghi `CAPTION_READY:314` ra stdout. Parent redirect cả hai stream nhưng gọi `WaitForExit(2500)` trước khi đọc. Child chưa thể hoàn tất khi stderr bị chặn, parent chưa bắt đầu consume.

## 3. Root cause
Redirected stdout/stderr là pipe có dung lượng hữu hạn. Parent đợi child exit trước khi drain, còn child phải ghi xong stderr mới exit: circular wait. Đây là process I/O ordering bug, không phải CPU bottleneck.

## 4. Why the fix works
Khởi động `ReadToEndAsync` đồng thời trên **cả hai stream** trước khi `await WaitForExitAsync`. Reader giải phóng pipe trong khi child ghi. Deadline/cancellation vẫn áp dụng; khi hủy, kill process tree rồi wait cleanup.

## 5. How to verify
```powershell
./verify.ps1
```
Script build và chạy **starter do người học sửa**, không chạy solution. Sau khi tự làm:
```powershell
Copy-Item ./starter/ConversionRunner.cs ./starter/ConversionRunner.mine.cs.bak
Copy-Item ./solution/ConversionRunner.cs ./starter/ConversionRunner.cs -Force
./verify.ps1
```
Bốn checks: exit 0, caption đúng, đủ 1 MiB diagnostic và không hỏng ký tự. Không sửa `Program.cs` để qua test.

## 6. Alternative fixes
- `BeginOutputReadLine` và `BeginErrorReadLine` cho output dạng dòng, nhưng phải quản lý event completion.
- Bounded pump hoặc spill-to-disk cho production; sau khi đạt quota vẫn phải **tiếp tục drain** để không tái tạo deadlock.
- Cho converter ghi diagnostic ra file nếu contract cho phép, kèm rotation, retention và access controls.

## 7. Wrong / tempting fixes
- Chỉ tăng timeout: không phá circular wait.
- Đọc stdout trước rồi mới stderr: có thể tiếp tục block khi stderr đầy.
- Bỏ redirect stderr: mất diagnostic và che lỗi vận hành.
- `Task.Run(() => WaitForExit())` nhưng không drain: chuyển thread chờ sang ThreadPool.
- Kill process ngay khi log nhiều: làm mất output hợp lệ.

## 8. Production implications
Giới hạn memory/log retention, tránh log PII và injection; phân biệt cancellation, timeout, non-zero exit và lỗi start. Dùng executable path allowlist, không ghép shell input; đo exit code, duration, stderr bytes, cleanup. Kiểm thử process tree trên Windows/Linux.

## 9. Trade-offs
`ReadToEndAsync` đơn giản, đúng cho fixture 1 MiB nhưng tăng memory theo output. Bounded stream pump + quota phức tạp hơn, thích hợp production. Timeout cần bao phủ cả exit và drain completion.

## 10. What a Senior engineer should notice
Parent/child có protocol I/O, không chỉ một lời gọi CLI. Verify output, exit, deadline và resource ownership; không chấp nhận chỉ “hết timeout”. Đây là static-review, chưa phải kết quả build/test.
