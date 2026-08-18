# Daily OpenAI

`Daily-OpenAI` là kho kiến thức cá nhân lưu các báo cáo, bài học, bản tin nghiên cứu và bộ công cụ AI được tạo từ các scheduled job hoặc lượt chạy thủ công trên ChatGPT.

Nội dung chủ yếu được viết bằng tiếng Việt và cá nhân hóa cho lập trình viên .NET Backend đang phát triển theo hướng Senior Developer, Technical Lead và Software Architect. Ví dụ tiếng Anh, tên nguồn, API, câu lệnh và code được giữ nguyên ngôn ngữ khi cần thiết.

> Repository này chỉ lưu kết quả đã sinh. Định nghĩa scheduled job, prompt và cấu hình chạy trên ChatGPT được quản lý bên ngoài repository.

## Nội dung repository

| Thư mục | Mục đích |
| --- | --- |
| `Daily .NET Job Opportunity Radar` | Các công việc .NET/C# đã được xác minh trong khoảng thời gian tìm kiếm quy định. |
| `Daily AI Engineering Kit` | Rules, skills, subagents, workflows, hooks và scripts có thể tái sử dụng cho AI-assisted engineering. |
| `Daily AI Role` | Bộ công cụ AI hoàn chỉnh mô phỏng một vai trò nghề nghiệp cụ thể. |
| `Daily AI Skills` | Các cập nhật mới về AI-assisted development và agent engineering. |
| `Daily Developer Tips & Design` | Năm developer tips và ba chủ đề software design trong mỗi bài. |
| `Daily GitHub Opportunity Radar` | Repository, release, issue có thể đóng góp và công cụ đáng thử nghiệm. |
| `Daily Interview & Code Review Challenge` | Câu hỏi phỏng vấn Senior .NET hoặc bài tập code review theo chế độ ngày chẵn/lẻ. |
| `Daily Knowledge Sharing` | Mười chủ đề software engineering từ nền tảng đến production trade-off. |
| `Daily Money Opportunity Scan` | Một cơ hội kiếm tiền có bằng chứng và có thể được một developer kiểm chứng. |
| `Daily Solo Builder Idea` | Một ý tưởng sản phẩm thực tế dành cho solo developer. |
| `Daily TOEIC Journey` | Lộ trình TOEIC liên tục dành cho người Việt mới bắt đầu. |
| `Daily Tech & Market Brief` | Tin quan trọng về AI, công nghệ, .NET, tài chính, startup và thị trường. |
| `Daily Workplace English` | Bài học tiếng Anh thực tế trong môi trường phát triển phần mềm. |

## Cách tổ chức output

Phần lớn scheduled job dạng báo cáo lưu một file Markdown cho mỗi lượt chạy thành công:

```text
{thư mục job}/{YYYY-MM-DD-HHmm}.md
```

- Thời gian được hiểu theo giờ Việt Nam (`UTC+7`).
- Timestamp đại diện cho lượt chạy đã tạo ra file.
- Khi cần giữ nhiều file lịch sử trùng tên, có thể dùng hậu tố `_2`, `_3`, v.v.
- `Daily AI Engineering Kit` và `Daily AI Role` dùng thư mục package vì mỗi bộ có thể chứa nhiều file liên quan.

Ví dụ:

```text
Daily Workplace English/2026-08-18-0200.md
Daily AI Role/incident-commander/
```

## Cách sử dụng

1. Mở thư mục tương ứng với nội dung muốn đọc.
2. Với báo cáo hằng ngày, sắp xếp filename theo timestamp và chọn lượt chạy phù hợp.
3. Với AI kit hoặc role package, bắt đầu từ `README.md` trong package nếu có.
4. Kiểm tra lại thông tin có tính thời điểm và external link trước khi đưa ra quyết định kỹ thuật, tài chính, nghề nghiệp hoặc kinh doanh.

Nội dung do AI tạo chỉ nên được dùng để học tập và nghiên cứu. Nội dung có thể thiếu, lỗi thời hoặc không chính xác và không thay thế tư vấn chuyên môn.

## Tài liệu

- [Tiêu chuẩn nội dung](docs/CONTENT-STANDARDS.md) mô tả output mong muốn của từng scheduled job.
- [Vận hành và bảo trì](docs/OPERATIONS.md) mô tả quy tắc đặt tên, kiểm tra và duy trì repository.

## License

Repository sử dụng giấy phép được mô tả trong [LICENSE](LICENSE).
