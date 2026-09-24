# UNIT-SEC-001 — Trusted Proxy Boundary Behind a Reverse Proxy

## Mục tiêu
Điều tra một incident trong API chạy sau reverse proxy, nơi link tuyệt đối do ứng dụng sinh ra thay đổi theo cách request đi vào hệ thống.

## Bối cảnh thực tế
Account Recovery API chạy sau reverse proxy. Monitoring cho thấy phần lớn recovery email có URL đúng, nhưng một số request đặc biệt tạo URL với hostname không thuộc hệ thống. API vẫn trả 200, token hợp lệ và không có exception rõ ràng.

## Bạn cần làm gì
Reproduce hiện tượng, thu thập evidence về request headers và URL được tạo, viết hypothesis, sửa starter để chỉ metadata từ boundary hợp lệ mới ảnh hưởng tới public URL, rồi chạy verification.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 7+ hoặc Windows PowerShell

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Script chạy hai request mô phỏng: một request đi qua proxy mong đợi và một request đi trực tiếp nhưng mang metadata giống proxy. So sánh recovery URL được sinh ra.

## Những gì cần quan sát
- Status code của cả hai request.
- Hostname trong recovery URL.
- Request path nào được coi là đến từ proxy.
- Evidence nào phân biệt trusted hop và direct client.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Thử fix trong `starter/`.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
[Reference Solution — spoiler](solution/README.md)

## Expected Results
- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time
45–60 phút.