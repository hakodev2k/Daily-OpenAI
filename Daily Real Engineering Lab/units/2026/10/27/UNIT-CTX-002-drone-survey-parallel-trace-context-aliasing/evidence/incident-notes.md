# Incident notes — Survey QA audit
- Đầu vào synthetic: 1, 2 và 6 tile. Mỗi tile ID là duy nhất.
- Không có dữ liệu bên ngoài, không có database và không có random input.
- Tổng audit rows đúng bằng số tile trong tất cả các batch.
- Một lần chạy có thể hiển thị:
  - expected=TILE-11, audit=TILE-12
  - expected=TILE-12, audit=TILE-12
- Giá trị audit chung ở các nhánh parallel không cố định giữa các máy/lần chạy.
- Các giả thuyết ban đầu cần phân biệt: sắp xếp kết quả, mất dữ liệu, lỗi ghi log, và sai correlation context.
- Batch gate được giữ cố định trong cả starter lẫn reference để tái hiện lỗi một cách kiểm soát.
