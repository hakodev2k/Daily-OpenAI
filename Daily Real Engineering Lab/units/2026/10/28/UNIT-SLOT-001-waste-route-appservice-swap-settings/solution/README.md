# Reference Solution — chỉ xem sau khi tự điều tra

## 1. Symptoms
Ngay sau khi staging được swap vào production, release label chuyển đúng sang `v2`, nhưng ứng dụng production đọc endpoint `sql-stage.internal`. Trên Azure thật, đây là một lỗi có thể dẫn đến truy cập sai dữ liệu; fixture này **không kết nối** database.

## 2. Evidence
Chạy `reproduce.ps1`: `production.ROUTE_DB_ENDPOINT` từ `sql-prod.internal` thành `sql-stage.internal`; trong khi `SLOT_ENVIRONMENT` vẫn là `production`. Sự khác biệt này loại trừ giả thuyết "toàn bộ cấu hình đều chuyển" và gợi ý cần xem metadata theo từng setting.

## 3. Root cause
Trong `starter/fixtures/slots.json`, `ROUTE_DB_ENDPOINT` ở cả production và staging có `slotSetting: false`. Theo mô hình App Service slot swap, app setting không đánh dấu slot-specific sẽ di chuyển sang slot đối diện. Endpoint database lại là cấu hình **gắn với môi trường**, không được đi theo release.

## 4. Reference configuration
So sánh [solution/fixtures/slots.json](fixtures/slots.json) với `starter/fixtures/slots.json`: chỉ đổi `ROUTE_DB_ENDPOINT.slotSetting` thành `true` tại **cả hai slot**. Không đổi giá trị endpoint, `RELEASE_LABEL`, `FEATURE_NEW_ROUTER` hoặc `SLOT_ENVIRONMENT`.

## 5. Why the fix works
`SlotSwapEngine.Swap` giữ giá trị setting có `slotSetting=true` tại slot gốc và chuyển các setting khác. Vì vậy production tiếp tục dùng database production trong khi release label `v2` và feature flag mới đi vào production. Kiểm tra cả hai phía ngăn cấu hình bất đối xứng.

## 6. Verification
Sao chép đúng thay đổi từ `solution/fixtures/slots.json` vào `starter/fixtures/slots.json`, sau đó chạy `.\verify.ps1`. Fixture xác nhận endpoint isolation, release movement, môi trường và rollback. `reproduce.ps1` vẫn tái hiện lỗi từ `baseline.json`. Trên Azure thật cần kiểm tra slot-setting flags trong portal/CLI, dry-run staging, kiểm tra database identity và telemetry sau swap; không chỉ tin vào việc endpoint trả HTTP 200.

## 7. Alternative fixes
- Giữ database endpoint theo slot bằng cấu hình slot-specific (lựa chọn trực tiếp cho lab).
- Tách hẳn cấu hình môi trường khỏi release artifact và quản lý bằng IaC có review, vẫn phải bảo đảm flags đúng khi swap.
- Không dùng swap cho hệ thống có ràng buộc dữ liệu quá phức tạp, thay bằng deployment workflow khác có kiểm soát, nhưng đánh đổi thời gian cutover và vận hành.

## 8. Wrong / tempting fixes
- **Đổi URL database staging thành production**: làm mất cô lập staging, có thể gây hỏng dữ liệu production trong test.
- **Đánh dấu mọi setting là sticky**: release label/feature flag không còn đi theo bản phát hành; deploy "thành công" nhưng chạy cấu hình cũ.
- **Swap lại ngay mà không điều tra**: có thể phục hồi tạm thời, không sửa rủi ro cho lần triển khai sau.
- **Chỉ kiểm tra HTTP health**: không xác nhận dữ liệu đến đúng môi trường.

## 9. Production implications
Quản lý slot-specific settings bằng IaC, kiểm tra đối xứng trên hai slot, dùng Managed Identity/Key Vault references phù hợp, xác nhận kết nối dữ liệu sau swap và có rollback runbook. Các cấu hình như connection strings, secrets, cache namespaces, queue endpoints cần được phân loại theo ranh giới môi trường; không phải mọi setting đều sticky.

## 10. Trade-offs
Sticky settings bảo vệ isolation nhưng tăng trách nhiệm duy trì từng môi trường; non-sticky settings thuận tiện chuyển release nhưng nguy hiểm nếu chứa giá trị đặc thù production. Chọn theo ownership của setting, không theo thói quen.

## 11. What a Senior engineer should notice
Sự cố không phải "build sai" mà là lỗi **deployment contract**. Cần deployment gate kiểm chứng invariants trước/sau swap, phát hiện drift giữa hai slot, thu thập evidence của effective config mà không log secret, và kiểm tra cả rollback. Fixture mô phỏng riêng quy tắc app settings, không chứng minh được mọi hành vi Azure thực.
