# Before — observable symptoms
Một số object còn sau deadline, một claim ACTIVE/held có object mất, và một worker có thể xử lý delete message cũ sau hold acceptance. Metadata DB không đủ để xác nhận physical deletion. Nếu không thấy, kiểm tra UTC+7, retention từ closedAt và nghĩa của RELEASED.
