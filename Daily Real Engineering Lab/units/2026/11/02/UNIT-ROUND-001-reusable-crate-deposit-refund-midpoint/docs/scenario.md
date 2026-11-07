# Hợp đồng đối soát tiền hoàn cọc

Hệ thống quản lý thùng tái sử dụng nhận `signedQuantity` (số nguyên có dấu) và `unitDeposit` (`decimal`, không âm). Số lượng dương là hoàn cọc khi trả thùng; số lượng âm là bút toán đảo một lần hoàn trước đó. Số lượng bằng 0 cho kết quả 0.

**Hợp đồng của đối tác:** nhân số lượng có dấu với tiền cọc đơn vị ở độ chính xác `decimal`, sau đó làm tròn **một lần trên tổng cuối cùng** về 2 chữ số thập phân. Khi số tiền nằm đúng giữa hai mức cent, chọn mức **có trị tuyệt đối lớn hơn**. Không làm tròn đơn giá trước phép nhân. Không dùng `double` cho tiền.

| signedQuantity | unitDeposit | Tổng chưa làm tròn | Kết quả hợp đồng |
|---:|---:|---:|---:|
| 1 | 1.005 | 1.005 | 1.01 |
| 3 | 0.335 | 1.005 | 1.01 |
| 1 | 1.015 | 1.015 | 1.02 |
| -1 | 1.005 | -1.005 | -1.01 |
| 1 | 2.674 | 2.674 | 2.67 |
| 1 | 2.676 | 2.676 | 2.68 |

## Phạm vi

Chỉ tính tiền hoàn cọc, không tính thuế, phí hoặc chuyển đổi ngoại tệ. Đầu vào `unitDeposit < 0` phải bị từ chối. Đừng thay đổi quy tắc để khớp kết quả hiện tại của starter: mục tiêu là bảo vệ hợp đồng nghiệp vụ.

## Evidence cần ghi

Ghi bảng input / tổng chưa làm tròn / actual / expected; xác định các giá trị nào tạo sai lệch. Sau đó so sánh các lựa chọn sửa về tính đúng đắn, tính tường minh của policy và rủi ro đổi quy tắc cho các tích hợp khác.
