# Investigation
1. Chạy `./reproduce.ps1`, lưu expected/actual và số U+FFFD.
2. Đọc fixture trong `harness/Program.cs`; breakpoint ở vòng đọc frame.
3. Thử ba giả thuyết: source bytes sai, JSONL framing sai, text transform sai.
4. Chứng minh/loại trừ từng giả thuyết bằng so sánh single-read, split-inside-character, one-byte-per-read.
5. Sửa duy nhất `starter/Utf8LineReader.cs`, chạy `./verify.ps1` và ghi kết quả.
6. Xem solution sau khi hoàn thành; đánh giá memory bound, error handling và telemetry.
