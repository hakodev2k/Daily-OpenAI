# Tiêu chuẩn nội dung

Tài liệu này mô tả ý nghĩa và điều kiện tối thiểu để chấp nhận nội dung trong `Daily-OpenAI`. Đây là hướng dẫn review, không phải nguồn cấu hình scheduled job trên ChatGPT.

## Tiêu chuẩn chung

Mỗi output nên:

- Sử dụng giờ Việt Nam (`UTC+7`) cho reporting window và delivery timestamp.
- Viết bằng tiếng Việt, ngoại trừ ví dụ tiếng Anh, code, câu lệnh, API hoặc technical term cần giữ nguyên.
- Đúng số lượng và cấu trúc mà job yêu cầu, không thêm nội dung chỉ để lấp chỗ trống.
- Phân biệt rõ fact đã xác minh, phân tích, giả định và recommendation.
- Dẫn trực tiếp tới nguồn primary hoặc authoritative khi nội dung phụ thuộc thông tin bên ngoài.
- Không tạo giả fact, ngày tháng, version, link, quotation, metric, job listing hoặc demand signal.
- Không lặp lại nội dung cũ nếu không có cập nhật đáng kể hoặc mục tiêu ôn tập rõ ràng.
- Hoàn chỉnh trước khi lưu; title-only, outline-only hoặc output bị cắt không được coi là lượt chạy hợp lệ.
- Dùng filename do runtime tạo. Model không được tự đoán output timestamp.

## Tiêu chuẩn theo từng job

| Job | Output tối thiểu được chấp nhận |
| --- | --- |
| **Daily .NET Job Opportunity Radar** | Liệt kê tất cả công việc .NET/C# duy nhất và có thể xác minh trong cửa sổ 72 giờ; loại Junior/Fresher, tin trùng, hết hạn và tin không xác định được thời gian; cung cấp direct job link cùng role, company, location, work type, stack và thời gian đăng/cập nhật nếu có. |
| **Daily AI Engineering Kit** | Tạo package có thể tái sử dụng với purpose, input, output, boundary và execution flow rõ ràng. Cung cấp rules, skills, subagents, workflows, hooks, scripts, schemas, examples hoặc tests phù hợp thay vì chỉ tạo cấu trúc rỗng. |
| **Daily AI Role** | Mô phỏng công việc thực tế của một role và tạo bộ rules, skills, subagents, workflows, hooks, scripts và knowledge phối hợp với nhau. Bộ công cụ phải hướng tới outcome rõ ràng, cường độ làm việc thực tế và khả năng dùng lại, không chỉ là demo một tác vụ. |
| **Daily AI Skills** | Ưu tiên thay đổi có ý nghĩa trong 24 giờ gần nhất. Nêu rõ điều gì thay đổi, vì sao quan trọng, giúp ích gì cho AI-assisted development và nên thử nghiệm thế nào. Claim mang tính thời điểm phải có direct source kèm ngày; kiến thức evergreen không được trình bày như tin mới. |
| **Daily Developer Tips & Design** | Có đúng năm developer tips và ba software-design topics. Mỗi mục nên giải thích problem, implementation, trade-off, mistake và production relevance ở mức Middle-to-Senior. |
| **Daily GitHub Opportunity Radar** | Bao phủ emerging repository, important release, contribution issue phù hợp và công cụ có thể áp dụng ngay. Dẫn tới đúng repository, release và issue cụ thể; không trả về danh sách project phổ biến chung chung. |
| **Daily Interview & Code Review Challenge** | Ngày chẵn theo giờ Việt Nam có đúng ba interview questions; ngày lẻ có đúng hai code-review challenges. Mỗi mục cần reasoning, expected answer hoặc corrected code, follow-up, common mistakes và evaluation guidance phù hợp mục tiêu Senior .NET/Architect. |
| **Daily Knowledge Sharing** | Có đúng mười software-engineering topics. Mỗi topic phải trình bày what, why, mechanics, implementation, example, production scenario, common mistakes, trade-offs, level progression và takeaway. Danh sách định nghĩa hoặc chỉ có tên chủ đề là không đủ. |
| **Daily Money Opportunity Scan** | Có đúng một cơ hội xuất phát từ demand hoặc inefficiency quan sát được. Phân biệt evidence với assumption và có target customer, value proposition, validation plan, smallest sellable solution, pricing hypothesis, risks cùng một hành động có thể làm ngay hôm nay. |
| **Daily Solo Builder Idea** | Có đúng một ý tưởng sản phẩm thực tế mà một developer có thể xây dựng và duy trì. Xác định problem, target user, MVP boundary, architecture direction, risks, validation approach và bước đầu tiên có thể triển khai. |
| **Daily TOEIC Journey** | Tạo một bài học 30–45 phút liên tục cho hồ sơ người Việt mới bắt đầu. Bài mới phải nối tiếp bài hợp lệ trước đó và có learn, practice, answers, mistake review, spaced repetition cho listening, reading, vocabulary, grammar, pronunciation và test skills. |
| **Daily Tech & Market Brief** | Ưu tiên diễn biến quan trọng trong 24 giờ gần nhất ở AI/technology, .NET/software, finance/macroeconomics và startups/business. Giải thích impact, phân biệt confirmed news với analysis, dùng direct reliable citation và không thêm tin giá trị thấp để đủ số lượng. |
| **Daily Workplace English** | Tạo một scenario 15–25 phút, có realistic dialogue hoặc response, năm đến tám useful phrases, vocabulary, một grammar pattern nhỏ, lỗi thường gặp của người Việt, pronunciation guidance, practice, active task và suggested answers. |

## Quy tắc progression của TOEIC

Journey hiện tại được reset vào `2026-08-17` theo giờ Việt Nam. Ngày này là Lesson 1, Week 1, Phase 1; lịch sử TOEIC trước ngày reset không thuộc progression hiện tại.

Mỗi lượt chạy sau đó phải:

1. Xác định bài hợp lệ mới nhất trong journey hiện tại.
2. Tạo đúng lesson tiếp theo (`previous lesson + 1`).
3. Không tính lesson number dựa trên ngày trong lịch.
4. Không tăng progress nếu bài hoàn chỉnh chưa được lưu thành công.
5. Chạy lại cùng một logical execution không được tạo lesson number thứ hai.

Các file lịch sử hiện có một số lesson number không tăng liên tục. Cho đến khi progression state được làm deterministic, người đọc nên kiểm tra lesson heading thay vì chỉ dựa vào filename.

## Checklist review

Trước khi chấp nhận một output, kiểm tra:

- File nằm trong đúng thư mục.
- Filename dùng đúng thời gian chạy thực tế theo giờ Việt Nam.
- Có đủ section và item count bắt buộc.
- Link mở đúng nguồn đang được trích dẫn.
- Claim có tính thời điểm nằm trong reporting window yêu cầu.
- Không lặp lại output trước đó nếu không có lý do rõ ràng.
- Markdown hoàn chỉnh và đọc được.
- Code, script, schema hoặc configuration nhất quán và có thể sử dụng.
