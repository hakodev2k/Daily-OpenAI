# Hint 3 — Cơ chế

`MemoryStream.GetBuffer()` trả về **backing array** có thể lớn hơn vùng đã ghi. Hãy bảo đảm transport nhận chính xác các byte có nghĩa, không phải toàn bộ vùng cấp phát. Kiểm tra `ToArray()` và phương án zero-copy dựa trên `TryGetBuffer()` + `ArraySegment.Offset/Count` (phải thay đổi contract và lifetime).
