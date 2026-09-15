# Progressive Hints

## Hint 1
Tách user-facing request lifetime khỏi lifetime mà business thực sự cho phép đối với công việc export.

## Hint 2
So sánh failure domain của ba option khi application deploy/restart giữa lúc export đang chạy.

## Hint 3
Đừng chỉ hỏi “có cần queue không?”. Hãy hỏi boundary nào đạt reliability cần thiết với operational complexity thấp nhất, và trigger nào sẽ buộc team tách worker sau này.