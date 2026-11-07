# Before — starter chưa sửa
- `reproduce.ps1` exit 0 nếu tái hiện: batch 1 = GS-101, batch 2/3 rỗng, tổng accepted = 1/4.
- `verify.ps1` exit khác 0 và in FAIL do thiếu ba event.
- Không dựa vào giá trị cursor tuyệt đối trên mọi runtime; so sánh nó với tiến độ nghiệp vụ.
