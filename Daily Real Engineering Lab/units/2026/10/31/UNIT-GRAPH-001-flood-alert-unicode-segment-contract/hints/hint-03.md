# Hint 03 — Khái niệm

Một `char` trong C# là UTF-16 code unit, không phải Unicode grapheme cluster và cũng không phải byte UTF-8. Cần cộng dồn theo text element; khi một element riêng lẻ vượt budget, hãy định nghĩa rõ contract từ chối.
