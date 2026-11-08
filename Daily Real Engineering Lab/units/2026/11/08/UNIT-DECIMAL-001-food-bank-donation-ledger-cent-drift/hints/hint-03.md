Hợp đồng là round từng giao dịch theo AwayFromZero rồi mới cộng. Thử `decimal.Parse(..., CultureInfo.InvariantCulture)` và `decimal.Round(value, 2, MidpointRounding.AwayFromZero)`.
