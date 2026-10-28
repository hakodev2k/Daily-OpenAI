# Evidence — output dự kiến của starter

> Mẫu sau được suy luận qua static review, **không phải log chạy thực tế**.

```text
Morning:   AM-EXAM|A01, AM-EXAM|A02
Afternoon: AM-EXAM|A01, AM-EXAM|A02, PM-EXAM|B03
REPRODUCED: independent-batch contract violated.
```

Input ca sáng: A01, A02; ca chiều: B03. Ca chiều chỉ được chứa session `PM-EXAM`.

`reproduce` trả 0 nếu quan sát được lỗi; `verify` trả 0 nếu lỗi được khắc phục và service graph hợp lệ.
