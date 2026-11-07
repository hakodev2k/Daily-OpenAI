# UNIT-CICD-002 — Build Once or Rebuild Per Environment?

## Mục tiêu
Đánh giá một release pipeline .NET và đưa ra quyết định về ranh giới build, artifact, configuration và promotion dựa trên evidence thay vì convention.

## Bối cảnh thực tế
Một team vận hành API qua Dev, Staging và Production. Mỗi environment hiện chạy pipeline từ cùng Git tag nhưng thực hiện restore/build/publish riêng. Staging đã pass integration test và smoke test; production deployment đôi khi tạo ra package có checksum khác. Team muốn biết liệu đây có phải vấn đề đáng sửa hay chỉ là khác biệt bình thường của pipeline.

## Bạn cần làm gì
Đọc scenario, ghi assumptions và decision trong `workspace/my-decision.md`. So sánh ít nhất ba phương án, nêu failure modes, rollback behavior, auditability, configuration handling và operational cost. Chọn một phương án phù hợp với constraints rồi giải thích migration path.

## Yêu cầu môi trường
Không cần cloud account hay tool đặc biệt. Chỉ cần Markdown editor và kiến thức CI/CD cơ bản.

## Chạy nhanh
1. Đọc `docs/scenario.md`.
2. Hoàn thành `workspace/my-decision.md` trước khi mở solution.
3. Dùng checklist trong scenario để tự review decision.

## Cách reproduce vấn đề
Dùng timeline và checksum evidence trong `docs/scenario.md` để mô phỏng release review. Xác định điều gì thực sự đã được Staging chứng minh và điều gì chưa được chứng minh cho Production.

## Những gì cần quan sát
- Quan hệ giữa source revision, dependency resolution, build environment và output artifact.
- Khả năng truy vết chính xác binary đã được test.
- Configuration nào thuộc artifact và configuration nào thuộc deployment/runtime.
- Rollback cần source rebuild hay chỉ cần chọn lại artifact.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix/decision.
4. Verify bằng constraints và failure scenarios.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
[Spoiler — chỉ xem sau khi hoàn thành decision](solution/README.md)

## Expected Results
Decision phải chỉ rõ artifact identity, promotion flow, environment configuration, rollback và trade-offs; không chỉ nói “dùng CI/CD best practice”.

## Estimated Time
45–60 phút.