# After — learner fix

verify.ps1 phải PASS cho 0, 1, 9 và 40 event; mọi sequence được ghi đúng một lần, theo thứ tự. Khi callback persist lỗi ở sequence 2, caller nhận exception thay vì success hoặc chờ vô hạn.

Không yêu cầu thời gian cố định. Dưới tải lớn, producer có thể phải đợi để bảo toàn dữ liệu. Nếu test treo, kiểm tra consumer có chạy trong lúc producer chờ và lỗi consumer có truyền tới producer hay không.
