# UNIT-ARCH-006 — Chọn boundary cho upload file lớn

## Mục tiêu

Thiết kế luồng upload file lớn cho một creator portal trên Azure với các ràng buộc thực tế về latency, memory, retry, security, antivirus scanning và chi phí vận hành.

Đây là Design Decision Lab. Không có một kiến trúc duy nhất đúng; bạn cần chọn một phương án có thể bảo vệ được bằng evidence và trade-off.

## Bối cảnh thực tế

Một creator portal hiện nhận file video qua ASP.NET Core API rồi stream tiếp sang Azure Blob Storage.

Hệ thống sắp tăng từ file trung bình 20 MB lên 2–8 GB. Team thấy các triệu chứng:

- API instances giữ connection rất lâu khi người dùng upload qua mạng chậm
- scaling API tăng mạnh trong giờ cao điểm
- retry từ client có thể gửi lại lượng dữ liệu rất lớn
- product yêu cầu antivirus scanning trước khi asset được publish
- security không muốn cấp quyền Blob rộng cho browser
- backend vẫn phải kiểm soát tenant, quota, metadata và trạng thái upload

## Bạn cần làm gì

So sánh ít nhất ba phương án:

1. client → ASP.NET Core API → Blob Storage
2. client → Blob Storage trực tiếp bằng quyền upload giới hạn, sau đó gọi finalize API
3. một biến thể async/staged khác mà bạn cho là hợp lý

Trong workspace/my-decision.md, ghi:

- assumptions
- trust boundaries
- failure modes
- retry semantics
- cách xác nhận upload hoàn chỉnh
- scanning workflow
- quota enforcement
- cleanup cho upload dở dang
- observability
- cost/operational trade-offs
- quyết định cuối cùng

## Yêu cầu môi trường

Không cần chạy code hoặc Azure subscription. Có thể vẽ sequence diagram bằng Markdown/Mermaid nếu muốn.

## Chạy nhanh

1. Đọc scenario và constraints.
2. Hoàn thành workspace/my-decision.md.
3. Chỉ sau đó xem solution/README.md để so sánh.

## Cách reproduce vấn đề

Không áp dụng. Đây là design decision lab, không phải executable failure lab.

## Những gì cần quan sát

Đừng mặc định "direct upload" hoặc "proxy qua API" là luôn đúng.

Hãy tách riêng:

- control plane: auth, quota, metadata, finalize, publish state
- data plane: bytes của file lớn
- failure boundary: upload incomplete, retry, duplicate finalize, scan failure
- security boundary: quyền nào được cấp, scope bao nhiêu, sống bao lâu
- operational boundary: API capacity, storage capacity, cleanup jobs

## Quy tắc làm lab

1. Ghi assumptions trước.
2. So sánh các phương án bằng cùng một bộ constraints.
3. Chọn phương án.
4. Nêu điều kiện khiến bạn đổi quyết định.
5. Chỉ sau đó mới xem reference solution.

## Hints

Không có hint bắt buộc. Nếu bị bí, bắt đầu bằng việc hỏi: "Có nhất thiết application server phải nằm trên đường đi của toàn bộ bytes không?"

## Reference Solution

[Spoiler — một phương án có thể bảo vệ được dưới các constraints đã cho](solution/README.md)

## Expected Results

Một câu trả lời tốt không chỉ vẽ boxes. Nó phải mô tả lifecycle từ create-upload-session → transfer → finalize → scan → publish/failed → cleanup.

## Estimated Time

45–75 phút.
