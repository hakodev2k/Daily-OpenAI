# Incident packet — dữ liệu giả lập

## Kiến trúc hiện tại
- PostgreSQL 15, bảng `fare_events(id uuid primary key, route_code text not null, amount_cents bigint not null, created_at timestamptz not null)`.
- V1 INSERT chỉ cung cấp bốn cột trên; V1 SELECT `amount_cents` để tính tổng.
- V2 mong đợi `gross_cents`, `adjustment_cents`, `currency_code` và tính net = gross + adjustment.
- Consumer V1 có thể đọc message cũ và ghi vào cùng bảng. Rolling deploy chạy 45 phút, có lúc 40% pods V1 và 60% pods V2.

## Đề xuất release đang bị tranh luận
10:00: `ALTER TABLE fare_events RENAME COLUMN amount_cents TO gross_cents;`
10:01: `ALTER TABLE fare_events ADD COLUMN currency_code text NOT NULL;`
10:02: deploy V2 (rolling).
10:45: tắt V1.

## Telemetry synthetic
| Thời điểm | V1 pods | V2 pods | INSERT errors/min | Consumer lag |
|---|---:|---:|---:|---:|
| 09:55 | 20 | 0 | 0 | 8m |
| 10:01 | 20 | 0 | 680 | 11m |
| 10:20 | 8 | 12 | 270 | 24m |
| 10:46 | 0 | 20 | 0 | 29m |

Tổng tiền trên dashboard là số cộng từ giao dịch đã được ghi thành công; dashboard không kiểm tra số sự kiện nguồn so với số dòng đã persist.

## Constraints
- Ingestion SLO: 99.95%; không được drop event.
- Khối lượng: 1.200 events/min; bảng 180 triệu dòng; storage còn 25%.
- Backfill được throttle tối đa 4.000 rows/s và phải resumable.
- V1 và V2 đồng tồn tại tối đa 45 phút theo dự kiến; consumer có thể trễ 30 phút.
- Rollback app trong 15 phút; DDL đảo ngược có thể nguy hiểm khi V2 đã ghi dữ liệu mới.
- Số tiền lưu theo cents, currency ban đầu đều VND, nhưng tương lai có thể thêm USD.
- Bất kỳ thay đổi destructive nào phải có sign-off và bằng chứng reconciliation.

## Nhiệm vụ thay đổi yêu cầu
Sau khi lập kế hoạch ban đầu, Ops xác nhận consumer có thể trễ **72 giờ**. Hãy sửa retention của compatibility layer, lịch contract và rollback gate. Giải thích vì sao chỉ đợi hết rolling deploy là không đủ.

## Dữ liệu cần tự tạo để đối soát
Chọn 5 event giả lập gồm: adjustment = 0, adjustment âm, adjustment dương, duplicate delivery và event V1 gửi trễ. Nêu expected net và expected unique count; không dùng thông tin cá nhân thật.
