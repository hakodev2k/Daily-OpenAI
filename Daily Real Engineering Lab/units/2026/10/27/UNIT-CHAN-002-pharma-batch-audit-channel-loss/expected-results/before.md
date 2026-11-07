# Before — starter chưa sửa

Với burst 5 event và capacity 2, kết quả dự kiến:

\`\`\`text
published=5 received=2
receivedSequences=[4,5]
producerCompletedBeforeConsumer=True
pendingProducerCancelled=False
completeOrderedDelivery=False
REPRODUCED: audit delivery contract broken
\`\`\`

\`./reproduce.ps1\` trả exit code 0 vì triệu chứng đã được tái hiện. \`./verify.ps1\` trả exit code 1. Nếu không thấy triệu chứng, kiểm tra rằng bạn đang chạy project \`starter/\` nguyên bản và chưa thay đổi cấu hình.
