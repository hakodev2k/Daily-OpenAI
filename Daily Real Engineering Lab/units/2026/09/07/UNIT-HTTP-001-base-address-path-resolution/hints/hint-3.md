# Hint 3

Với base `https://example.test/api/`, relative `orders/42` được append dưới `/api/`. Nếu base kết thúc bằng `api` không có trailing slash, segment cuối có thể bị coi là phần cần replace khi resolve relative URI.
