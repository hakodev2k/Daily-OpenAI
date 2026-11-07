# Hint 03 — Hướng kỹ thuật

Thử `System.Net.Http.Headers.StringWithQualityHeaderValue.TryParse`. Chỉ chọn trong tập locale hỗ trợ; phân biệt `q=0` với `q>0`, xử lý subtag vùng và wildcard. Test cả trường hợp trả về `null` (API có thể phản hồi 406).
