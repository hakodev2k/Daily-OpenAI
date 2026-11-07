# Before — starter baseline
`reproduce.ps1` phải PASS khi chưa sửa starter. `verify.ps1` phải FAIL trên starter ban đầu.

- Session `operator-17` chứa `A-poison,B-ok,A-next`; `operator-88` chứa `C-ok,D-ok`.
- `A-poison` thất bại trong cycle 1–4, chuyển DLQ ở cycle 4.
- `B-ok` được nhận ở cycle 5, dù nó thuộc charger khỏe.
- `A-next` vẫn pending sau cycle 5; `C-ok` cycle 1 và `D-ok` cycle 2.
- Đây là deterministic simulation; không suy ra độ trễ production theo giây hay đặc tính scheduling của Azure Service Bus.
