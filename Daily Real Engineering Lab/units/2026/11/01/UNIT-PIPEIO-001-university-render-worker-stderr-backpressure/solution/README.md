# Reference Solution — SPOILER

Chỉ đọc sau khi reproduce và thử sửa. Mã tham khảo ở [Program.cs](Program.cs). scripts/verify.ps1 không sử dụng bản này.

## 1. Symptoms

Stderr 128 ký tự: worker trả STREAMS_DRAINED, exit 0. Stderr 524288 ký tự: worker hết hạn 4 giây, giết renderer và trả exit 23. Không có bằng chứng renderer tính toán chậm.

## 2. Evidence

Renderer con ghi nhiều ký tự E vào stderr trước khi ghi rendered=N vào stdout. Tiến trình cha đợi đọc stdout đến EOF. Khi stderr pipe đầy, renderer bị chặn ở thao tác ghi; không thể đóng stdout hoặc kết thúc. Cha chưa đọc stderr vì đang chờ stdout. CPU thấp phù hợp với chờ I/O.

## 3. Root cause

Hai redirected pipe là bộ đệm hữu hạn và độc lập. ReadToEnd() tuần tự stdout rồi stderr tạo vòng chờ: cha chờ stdout EOF, con chờ stderr có chỗ trống. Task.Run chỉ chuyển thao tác chờ sang ThreadPool, không làm hai stream được tiêu thụ đồng thời.

## 4. Why the fix works

Bắt đầu ReadToEndAsync() cho cả stdout và stderr trước khi đợi WaitForExitAsync(). Task.WhenAll theo dõi hai luồng và vòng đời tiến trình. Khi pipe được drain đồng thời, stderr không làm con nghẽn vô hạn. Giữ deadline, kill process tree khi timeout, thu hồi task và kiểm tra exit code cùng độ dài output.

## 5. How to verify

Sao chép cách sửa vào starter/Program.cs, chạy pwsh -File ./scripts/verify.ps1. Kiểm tra stderr 128, 131072, 524288 với exit 0; thêm renderer trả exit 7. Mong đợi STREAMS_DRAINED cho ba case đầu, CHILD_FAILED/31 cho case cuối. reproduce.ps1 sau khi sửa sẽ FAIL đúng thiết kế vì lỗi gốc không còn.

## 6. Alternative fixes

- BeginOutputReadLine/BeginErrorReadLine hợp lý với log theo dòng, nhưng phải xử lý payload không có newline.
- Stream theo chunk ra file tạm để không giữ toàn bộ log lớn trong RAM, kèm byte cap và cleanup.
- Không redirect stream không cần thiết, nhưng chỉ khi audit contract cho phép.

## 7. Wrong or misleading fixes

- Tăng timeout từ 4 lên 60 giây chỉ kéo dài vòng chờ.
- Tăng ThreadPool min threads không giải quyết quan hệ chờ giữa OS pipes.
- Chỉ chuyển ReadToEnd vào Task.Run vẫn giữ thứ tự tiêu thụ sai.
- Bỏ stderr làm mất bằng chứng audit.
- Luôn trả exit 0 che giấu lỗi renderer.

## 8. Production implications

Giới hạn byte log, redact dữ liệu nhạy cảm, timeout tổng và shutdown riêng, correlation ID, phân biệt timeout/nonzero-exit/invalid-output và metrics theo nguyên nhân. Không ghép command shell từ input; sử dụng ArgumentList.

## 9. Trade-offs

ReadToEndAsync đơn giản với payload nhỏ nhưng buffer toàn bộ trong RAM. Với log lớn, dùng bounded streaming hoặc file tạm, byte cap và backpressure. Cân nhắc huỷ tiến trình khi caller cancel.

## 10. Senior engineer should notice

Timeout là triệu chứng, không phải root cause. Phải hiểu process topology, OS pipe buffers, byte flow, lifecycle và exit-code contract. Kiểm thử payload vượt pipe buffer, không phụ thuộc kích thước pipe cố định.

## Tài liệu chính thức

- [ProcessStartInfo.RedirectStandardError](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.processstartinfo.redirectstandarderror)
- [Process.StandardOutput](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.process.standardoutput)
- [Process.WaitForExitAsync](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.process.waitforexitasync)
