# Hint 3
Tạo một cancellation scope cho toàn operation với 900ms deadline, liên kết nó với `requestAborted`, rồi truyền cùng token đó xuống mọi dependency call.