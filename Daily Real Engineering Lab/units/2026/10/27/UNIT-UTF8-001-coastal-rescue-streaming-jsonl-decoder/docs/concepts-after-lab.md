# Concepts after lab
- TCP/HTTP stream có thể cắt tại mọi byte; `ReadAsync` không tương ứng message hay Unicode character.
- UTF-8 có thể mã hóa một scalar bằng 1–4 byte. Decoder state phải tồn tại qua các lần đọc.
- `Encoding.UTF8.GetString` độc lập trên từng frame có thể sinh U+FFFD và JSON vẫn parse thành công.
- `Decoder.GetChars` với `flush:false` giữ sequence dở; `flush:true` kết thúc luồng. `UTF8Encoding(false,true)` từ chối wire không hợp lệ.
- Chỉ tách JSONL theo newline sau khi giải mã; cần giới hạn kích thước line, kiểm tra EOF và metrics lỗi.
- Production: mỗi connection có decoder riêng; cân nhắc `StreamReader` hoặc `System.IO.Pipelines` khi cần throughput và backpressure.

Microsoft Learn: https://learn.microsoft.com/dotnet/api/system.text.decoder.getchars ; https://learn.microsoft.com/dotnet/api/system.text.utf8encoding ; https://learn.microsoft.com/dotnet/standard/base-types/character-encoding-introduction
