# UNIT-CFLIGHT-001 — Solar Subsidy Tariff Cache: Expiry Burst Decision

## Mục tiêu

Điều tra một sự cố khuếch đại lưu lượng ở lớp tra cứu biểu giá, thu thập bằng chứng và bảo vệ hợp đồng concurrency mà không đánh đổi tính độc lập giữa các vùng.

## Bối cảnh thực tế

Cổng đăng ký trợ giá lắp đặt điện mặt trời cho hộ dân dùng biểu giá theo vùng từ một nhà cung cấp có quota 4 request/giây. Trên dashboard, phần lớn thời gian phản hồi tốt, nhưng một số đợt traffic đồng thời khiến số call tới đối tác tăng đột biến và có nguy cơ bị 429. Đội vận hành muốn một thay đổi nhỏ, có thể kiểm chứng trước khi xem xét hạ tầng phân tán.

## Bạn cần làm gì

- Chạy bản starter, ghi số lần gọi upstream cho 12 request đồng thời cùng vùng.
- Phân biệt đúng dữ liệu trả về với hiệu quả sử dụng tài nguyên.
- Đưa ít nhất hai giả thuyết và chọn một phương án đáp ứng yêu cầu.
- Sửa **starter/TariffCache.cs**; không sửa contract, scenario hay script để làm test pass.
- Viết quyết định trong [workspace/decision.md](workspace/decision.md), ghi kết quả tại [workspace/my-investigation.md](workspace/my-investigation.md).

## Yêu cầu môi trường

Windows PowerShell 7+ hoặc Windows PowerShell 5.1, .NET SDK 8.0+ có khả năng build target net8.0. Không cần Redis, Docker, database hay Azure.

## Chạy nhanh

Tại thư mục unit:

```powershell
./scripts/setup.ps1
./scripts/run.ps1
```

## Cách reproduce vấn đề

```powershell
./scripts/reproduce.ps1
```

Script thành công khi starter thể hiện đúng triệu chứng đã biết. Sau khi tự sửa, chạy:

```powershell
./scripts/verify.ps1
```

Script verify kiểm tra **starter do bạn chỉnh**, không kiểm tra hộ reference solution.

## Những gì cần quan sát

Ghi lại WarmCacheHit, BurstProviderCalls, AllQuotesAgree, IndependentKeysOverlap và FailedFetchCanRetry. Với 12 client cùng vùng, mục tiêu là tối đa **một** lượt gọi upstream trong một đợt tra cứu, trong khi hai vùng khác nhau vẫn có thể xử lý đồng thời. Kết quả trả về phải nhất quán; lỗi tạm thời phải cho phép retry.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler:** Chỉ mở [solution/README.md](solution/README.md) sau khi tự điều tra và verify.

## Expected Results

[Trước](expected-results/before.md) · [Sau](expected-results/after.md). Không cần khớp thời gian thực thi tuyệt đối trên mọi máy.

## Estimated Time

55–70 phút · L5 Design Decision / D4 Production Troubleshooting. Xem [prerequisite](docs/prerequisite.md) trước khi bắt đầu và [concepts](docs/concepts-after-lab.md) sau khi làm xong.
