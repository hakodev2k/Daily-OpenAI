# Before — Starter

Kỳ vọng từ static review (chưa chạy SDK): en-US valid control PASS; de-DE và một số locale khác diễn giải token dấu chấm khác nhau hoặc báo lỗi; input `1,234` có thể bị nhận sai. `reproduce.ps1` trả 0 nếu chứng minh lỗi; `verify.ps1` phải trả khác 0 trên starter nguyên bản.

Nếu không reproduce, kiểm tra SDK, locale fixture và đúng project `starter/`. Không chạy reference solution thay starter để xác nhận bug.
