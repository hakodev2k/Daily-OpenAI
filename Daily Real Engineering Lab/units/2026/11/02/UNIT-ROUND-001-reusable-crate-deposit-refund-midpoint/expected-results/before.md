# Before — Starter

Chạy `./reproduce.ps1` phải in hai sai lệch hợp đồng, đồng thời kết thúc exit code 0 vì đã **reproduce được lỗi**.

```text
Single: amount=1.005, actual=1.00, contract=1.01
Batch:  amount=3*0.335, actual=1.00, contract=1.01
Control: amount=1.015, actual=1.02, contract=1.02
REPRODUCED: two partner settlement contract mismatches.
```

`./verify.ps1` phải thất bại ở ít nhất ca `single midpoint` khi chưa sửa. Nếu reproduce không ra triệu chứng, xác nhận bạn đang chạy đúng `starter/Refund.csproj` và chưa thay đổi code.
