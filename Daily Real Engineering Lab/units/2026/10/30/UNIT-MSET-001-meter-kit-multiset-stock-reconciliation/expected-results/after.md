# After
`./verify.ps1` exit 0 và in `Verification: 6/6 passed.` Báo thiếu một `KIT-A`, hai `KIT-X`, thừa hai `KIT-Z`; không báo sai khi hoán vị; giữ hợp đồng SKU phân biệt hoa/thường. Sau khi sửa, `./reproduce.ps1` exit 1 vì triệu chứng gốc không còn.
