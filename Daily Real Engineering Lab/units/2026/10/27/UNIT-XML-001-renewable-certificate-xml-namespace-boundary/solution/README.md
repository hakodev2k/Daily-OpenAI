# Reference Solution — chỉ xem sau khi tự sửa

## 1. Symptoms

Batch `OCT-27` được báo thành công nhưng `readingCount=0` và `totalKwh=0`; retry cùng payload vẫn ra kết quả giống nhau.

## 2. Evidence

XML hợp lệ và có hai số đo dưới `Settlement/Meter/Reading`. Các phần tử thuộc namespace của hợp đồng không khớp với truy vấn chuỗi đơn giản. Trong cùng document có thêm `Diagnostics/Reading` và `ext:Reading` không phải dữ liệu đối soát.

## 3. Root cause

LINQ to XML sử dụng **expanded names** (namespace URI + local name). Truy vấn `Descendants("Reading")` chỉ tìm node không thuộc namespace, vì vậy bỏ qua số đo thuộc default namespace. Đổi thành tìm mọi `LocalName == "Reading"` cũng không đúng: nó trộn cả diagnostics và extension.

## 4. Why the fix works

[CertificateImporter.cs](CertificateImporter.cs) xác thực root contract version bằng `XNamespace`, sau đó chỉ duyệt đường dẫn trực tiếp `Settlement/Meter/Reading` trong namespace nghiệp vụ. Prefix trong XML chỉ là alias; URI mới quyết định identity. Thuộc tính `kwh` không có prefix nên vẫn dùng `Attribute("kwh")`. Số thập phân được parse bằng `InvariantCulture`.

## 5. Verification

Sửa file `starter/CertificateLab/CertificateImporter.cs` theo cách hiểu của bạn; chạy `./verify.ps1`. Script luôn build và chạy starter/workspace, **không** build file solution. Regression checks bao gồm hai payload tương đương có prefix khác nhau, dữ liệu chẩn đoán không được đếm, version không hỗ trợ bị từ chối và thiếu thuộc tính bị từ chối.

## 6. Alternative fixes

- `XmlReader` với kiểm tra `NamespaceURI` và path state: phù hợp file rất lớn, ít allocation hơn, nhưng cần thiết kế state machine và validation kỹ hơn.
- XSD validation ở ingress: hữu ích nếu có schema đáng tin cậy, nhưng không thay thế lựa chọn dữ liệu đúng scope.
- Adapter riêng cho từng phiên bản schema: hợp lý khi v1/v2 phải cùng được hỗ trợ, cần contract tests riêng.

## 7. Wrong / tempting fixes

- `Name.LocalName == "Reading"`: làm test đơn giản pass nhưng đếm cả node chẩn đoán hoặc namespace khác.
- Xóa `xmlns` khỏi XML bằng string replace: phá hỏng identity và có thể thay đổi ý nghĩa dữ liệu đối tác.
- Chấp nhận tổng 0 là mặc định: biến lỗi ingest thành sai lệch đối soát âm thầm.
- Retry, tăng timeout hoặc scale-out: không sửa được lỗi deterministic trong cách chọn node.

## 8. Production implications

Ghi log `contractVersion`, `batchId`, `selectedReadingCount`, trạng thái reject và correlation ID; cảnh báo khi batch có cấu trúc hợp lệ nhưng số bản ghi thấp bất thường. Không ghi nguyên payload nhạy cảm vào logs. Giới hạn kích thước XML, cấm DTD/external entity theo threat model, đặt schema validation và xử lý version ở ranh giới ingress.

## 9. Trade-offs

`XDocument` giúp debug và review dễ, phù hợp batch nhỏ; với batch lớn, cần cân nhắc streaming `XmlReader` để tránh tải toàn bộ DOM. Strict rejection giúp ngăn sai số âm thầm nhưng có thể làm tăng tỷ lệ lỗi khi đối tác rollout schema mới; phối hợp version negotiation và quan sát thay đổi hợp đồng.

## 10. Senior engineer should notice

Không sửa riêng triệu chứng count=0. Cần xác định contract, phân biệt metadata/diagnostics với business facts, đặt invariant cho kết quả, và bảo vệ import bằng positive/negative contract fixtures. Sau đó mới tối ưu performance.

## Official references

- Microsoft Learn — LINQ to XML namespaces: https://learn.microsoft.com/en-us/dotnet/standard/linq/namespaces-overview
- Microsoft Learn — `XNamespace`: https://learn.microsoft.com/en-us/dotnet/api/system.xml.linq.xnamespace
- Microsoft Learn — `XContainer.Elements`: https://learn.microsoft.com/en-us/dotnet/api/system.xml.linq.xcontainer.elements
