# Hint 3

Trong simulator, `Delivery.RenewLock()` gia hạn ownership. Trong Azure Service Bus, cùng ý tưởng tồn tại qua lock renewal / auto lock renewal. Hãy duy trì lock theo chu kỳ ngắn hơn lock duration trong suốt phần xử lý dài, rồi complete sau khi side effect thành công.
