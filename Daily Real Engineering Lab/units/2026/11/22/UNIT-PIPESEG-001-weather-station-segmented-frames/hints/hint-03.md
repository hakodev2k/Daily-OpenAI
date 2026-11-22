# Hint 3
Một `ReadOnlySequence<byte>` có thể chứa nhiều segment. Để xử lý frame nhỏ có giới hạn kích thước, cân nhắc gom đầy đủ payload trước khi decode; với frame lớn, dùng `SequenceReader<byte>` để đọc qua các ranh giới segment.
