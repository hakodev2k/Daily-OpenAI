# After — contract đạt

Sau khi sửa, mong đợi:

\`\`\`text
published=5 received=5
receivedSequences=[1,2,3,4,5]
producerCompletedBeforeConsumer=False
pendingProducerCancelled=True
completeOrderedDelivery=True
VERIFIED: delivery, backpressure and cancellation
\`\`\`

\`./verify.ps1\` phải trả exit code 0 trên project \`starter/\` do bạn chỉnh sửa. \`./reproduce.ps1\` sẽ không còn tìm thấy triệu chứng cũ và có thể trả exit code 1; đây là hành vi đúng sau khi sửa.

Lưu ý: test chỉ chứng minh delivery trong tiến trình đang chạy, **không** chứng minh durability sau crash hoặc exactly-once giữa các service.
