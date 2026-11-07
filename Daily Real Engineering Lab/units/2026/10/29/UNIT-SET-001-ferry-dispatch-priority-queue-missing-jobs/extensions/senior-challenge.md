# Senior extension
Thiết kế API Enqueue từ ba producer song song có ReceivedSequence riêng. Làm rõ uniqueness theo JobId, thứ tự toàn cục và ack sau durable commit. Đề xuất test race và crash/restart; không sửa fixture chính trước acceptance.
