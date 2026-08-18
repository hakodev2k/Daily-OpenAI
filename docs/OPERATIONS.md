# Vận hành và bảo trì

Repository này là nơi lưu output của nhiều scheduled job và manual run trên ChatGPT. Scheduling, browser automation và prompt configuration được quản lý bên ngoài repository; tài liệu này mô tả cách ghi và duy trì kết quả tại đây.

## Luồng tạo output

Một lượt chạy thành công của job dạng báo cáo nên thực hiện theo thứ tự:

1. Execution layer xác định thời gian hiện tại theo giờ Việt Nam (`UTC+7`).
2. Thu thập context, output trước đó và external evidence cần thiết.
3. Tạo đầy đủ nội dung theo yêu cầu riêng của job.
4. Kiểm tra kết quả theo [CONTENT-STANDARDS.md](CONTENT-STANDARDS.md).
5. Ghi output vào đúng thư mục.
6. Chỉ commit sau khi file đã hoàn chỉnh.

Runtime phải tạo filename và delivery metadata. Không dùng nội dung do model sinh làm nguồn xác định thời gian hiện tại.

## Quy tắc đặt tên

### Thư mục job

- Dùng official display name của scheduled job.
- Dùng khoảng trắng giữa các từ; không tạo thêm một bản thư mục dùng dấu gạch nối.
- Giữ nguyên capitalization để cùng một job luôn ghi vào cùng một path.

### File báo cáo

```text
YYYY-MM-DD-HHmm.md
```

Giá trị phải là timestamp thực tế của lượt chạy theo giờ Việt Nam. Nếu hợp nhất dữ liệu lịch sử và có hai file khác nhau trùng tên, giữ cả hai bằng hậu tố `_2`, `_3`, v.v.

### Generated package

`Daily AI Engineering Kit` và `Daily AI Role` sử dụng package name dạng `kebab-case`:

```text
Daily AI Role/incident-commander/
Daily AI Engineering Kit/ai-pr-review-gate/
```

Mỗi package nên có `README.md` riêng mô tả purpose, inputs, outputs, components, execution flow, configuration và validation.

## Validation và xử lý lỗi

Không tạo hoặc commit output khi:

- Thiếu section hoặc item count bắt buộc.
- Claim cần nghiên cứu nhưng không có nguồn sử dụng được.
- Output chỉ là outline hoặc bị cắt giữa chừng.
- Destination directory không khớp job.
- Logical run đã được lưu trước đó.
- Progressive job không xác định được đúng next state.

Với lỗi có thể phục hồi, retry việc tạo nội dung mà không tăng job state. Với lỗi không thể phục hồi, ghi lỗi trong runner log thay vì commit một file Markdown chưa hoàn chỉnh.

## Chống duplicate run

Writer nên lưu execution identifier hoặc idempotency key tương đương. Trước khi ghi file:

1. Tạo stable key từ job ID và scheduled occurrence.
2. Kiểm tra key đã hoàn thành hay chưa.
3. Lưu và commit output.
4. Chỉ đánh dấu key hoàn thành sau khi commit thành công.

Manual run cần execution identifier riêng.

## Progressive content

Các job như `Daily TOEIC Journey` phụ thuộc state trước đó. Nên dùng state tường minh, ví dụ:

```json
{
  "journeyStartedAt": "2026-08-17",
  "currentLesson": 55,
  "lastCompletedFile": "2026-08-18-0152.md"
}
```

Chỉ cập nhật state sau khi commit nội dung tương ứng thành công. Thay đổi tài liệu này không thêm hoặc sửa runtime state; ví dụ trên mô tả hành vi mong muốn trong tương lai.

## Thêm scheduled job mới

Khi thêm job mới:

1. Chọn một canonical directory name dùng khoảng trắng.
2. Mô tả purpose và minimum accepted output trong README gốc và content standards.
3. Xác định job tạo timestamped report hay multi-file package.
4. Xác định source, freshness, structure và deduplication requirements.
5. Thêm validation trước khi bật automatic commit.
6. Chạy thử thủ công một lần và review file đã commit.

## Bảo trì repository

- Ưu tiên pull request cho directory move, bulk rename và documentation change.
- Không âm thầm ghi đè historical content không liên quan.
- Khi hợp nhất thư mục, giữ file trùng tên bằng numeric suffix.
- Kiểm tra internal Markdown link sau khi đổi tên thư mục.
- Coi generated research là time-sensitive và giữ timestamp để tra cứu lịch sử.
- Kiểm tra định kỳ truncated file, duplicate run, broken link và progressive numbering không nhất quán.
