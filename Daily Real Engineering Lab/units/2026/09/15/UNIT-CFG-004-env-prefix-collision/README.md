# Lab: Configuration prefix collision

Một .NET worker đọc configuration từ environment variables. Local chạy đúng, nhưng deployment có thêm biến môi trường dùng chung và application đọc nhầm giá trị cho một setting quan trọng.

Chạy starter, quan sát effective configuration, xác định boundary giữa application-specific settings và host-wide environment, rồi sửa configuration loading để contract rõ ràng và có thể kiểm chứng. Ghi investigation vào `workspace/my-investigation.md`, chạy `scripts/verify.ps1`, sau đó so sánh với reference solution.

Hints: kiểm tra thứ tự configuration providers và cách environment-variable prefix được áp dụng. Không hard-code giá trị production vào source.
