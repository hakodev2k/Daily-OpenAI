# Before — starter gốc

```text
page1=106,105,104
inserted=107
page2=104,103,102
overlap=104
original_seen=102,103,104,105,106
```

`reproduce.ps1` phải PASS (exit 0) vì triệu chứng đã tái hiện. `verify.ps1` phải FAIL: cursor không mang anchor, trang sau bị lặp, trường hợp delete và tie không đạt contract. Nếu không thấy output trên, kiểm tra đang ở starter chưa sửa.
