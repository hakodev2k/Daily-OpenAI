# Hint 3
Với ORDER BY CreatedAt DESC, DonationId DESC, điều kiện trang sau là CreatedAt < @lastCreatedAt OR (CreatedAt = @lastCreatedAt AND DonationId < @lastId).
