# After — hợp đồng hành vi

Chạy `./scripts/verify.ps1` sau khi sửa `starter/CaptionLocale.cs`:
- Tất cả 16 case đều `PASS`, dòng cuối `VERIFIED`, exit code 0.
- Các lựa chọn có trọng số, loại trừ, ngôn ngữ vùng, wildcard và tie được xử lý theo hợp đồng của lab.
- Không có header hoặc header rỗng: mặc định `en`.
- Không có locale phù hợp: trả `null` để lớp HTTP có thể phản hồi `406 Not Acceptable`.
- `reproduce.ps1` vẫn xác nhận lỗi ban đầu vì chạy baseline bất biến.
- Không sửa `harness/Program.cs` để làm xanh test.
