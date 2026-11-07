# After — acceptance criteria

- ordinary: PASS, actual ORD-01,ORD-02,ORD-03.
- boundary: PASS, actual NEG,POS.
- tie: PASS, actual A-1,Z-9.
- ./verify.ps1 trả exit code 0 và in VERIFIED trên code do học viên sửa trong starter/.
- Kết quả không phụ thuộc vào thứ tự ban đầu của các phần tử có cùng DeltaMinorUnits.
- Không đổi expected fixtures, không ẩn failure bằng catch chung hay lọc bỏ record.
