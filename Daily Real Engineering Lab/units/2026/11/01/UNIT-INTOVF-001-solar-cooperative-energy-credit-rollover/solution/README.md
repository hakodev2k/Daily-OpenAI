# Reference Solution — chỉ xem sau khi tự sửa

## 1. Triệu chứng
Lô nhỏ trả đúng; khi tổng sản lượng vượt một ngưỡng, kết quả từ dữ liệu hoàn toàn dương có thể thành âm. `reproduce.ps1` trên starter ban đầu báo sai lệch, `verify.ps1` thất bại.

## 2. Bằng chứng
Mỗi điểm đo là 65.000 Wh. 33.038 điểm đo cho 2.147.470.000 Wh, vẫn trong `Int32`. 33.039 điểm đo cho 2.147.535.000 Wh, vượt `Int32.MaxValue = 2.147.483.647`. Lô 48.000 điểm đo phải là 3.120.000.000 Wh.

## 3. Root cause
Biến `subtotalWh` của starter là `int`. Phép `subtotalWh += reading.GeneratedWh` được thực hiện bằng số học `Int32`; trong ngữ cảnh `unchecked` mặc định, giá trị vượt ngưỡng bị wrap. Chỉ sau đó kết quả sai mới được chuyển sang kiểu trả về `long`. Kiểu trả về lớn không sửa được dữ liệu đã sai ở phép cộng trước đó.

## 4. Vì sao fix hoạt động
[Code tham khảo](EnergySettlement.cs) khai báo accumulator `long` ngay từ đầu, vì thế từng phép cộng được thực hiện bằng `Int64`. Khối `checked` giúp fail-fast nếu về sau tổng vượt cả `Int64`, thay vì trả giá trị âm im lặng. Vẫn giữ validation số đo âm.

## 5. Cách verify
Chỉ thay `starter/EnergySettlement.cs`, sau đó chạy `./verify.ps1`. Script chạy chính starter đã sửa và xác nhận lô rỗng, lô nhỏ, hai phía ranh giới, lô lớn, tính lặp lại và validation đầu vào. Trên starter gốc, script phải fail. `reproduce.ps1` chỉ dành cho chứng minh lỗi ban đầu.

## 6. Alternative fixes
- Có thể dùng `Enumerable.Sum(x => (long)x.GeneratedWh)` nếu vẫn giữ validation và xử lý `null` đúng; ép kiểu trước khi cộng là điểm cốt lõi.
- `decimal` có thể phù hợp nếu bài toán chuyển sang tiền tệ hoặc năng lượng có phần thập phân, nhưng tăng chi phí và không cần thiết cho Wh nguyên ở đây.
- Phân vùng tính toán theo lô là hợp lý cho dữ liệu cực lớn, nhưng tổng trung gian và tổng cuối vẫn phải có kiểu đủ rộng.

## 7. Wrong / Tempting Fixes
- Đổi riêng kiểu trả về thành `long` không thay đổi kiểu phép cộng.
- `(long)subtotalWh` ở cuối chỉ chuyển giá trị đã sai.
- Dùng `Math.Abs` với tổng âm che mất lỗi và có thể tạo số sai khác; `Math.Abs(int.MinValue)` còn có thể ném exception.
- Bỏ những điểm đo cuối cùng để tổng nhỏ đi làm mất dữ liệu nghiệp vụ.
- Dùng `unchecked` để tránh exception không bảo đảm correctness; ở đây cần ngăn overflow ngay từ đầu.

## 8. Production implications
Giám sát invariant: tổng không âm nếu mọi số đo không âm; cảnh báo khi sai. Kiểm tra ngưỡng theo dữ liệu lớn thực tế, không chỉ bộ test nhỏ. Nếu báo cáo dùng Wh rồi quy đổi sang kWh hay tiền, xác định quy tắc làm tròn và độ chính xác riêng cho từng bước.

## 9. Trade-offs
`long` đơn giản, nhanh và đủ cho miền giá trị này. `checked` thêm một kiểm tra để lỗi rõ ràng, nhưng cần xử lý exception ở boundary của batch job và không nên phát hành báo cáo chưa hoàn tất. Không tự động đổi toàn hệ thống sang `decimal` khi chưa có yêu cầu phân số.

## 10. Senior engineer cần nhận ra
Hãy xem xét kiểu **từng biểu thức trung gian**, không chỉ schema đầu vào/đầu ra. Một bug chỉ xuất hiện khi dữ liệu lớn thường cần boundary tests, invariant, và chứng minh bằng số cụ thể trước khi đưa ra thay đổi kiến trúc. Đừng coi việc đổi kiểu số là đủ nếu downstream serialization, database column hoặc UI vẫn có giới hạn nhỏ hơn.

## Tài liệu
Xem [references](../docs/references.md).
