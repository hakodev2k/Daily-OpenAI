# Senior extension
Thiết kế download xuống file .partial với ETag mạnh, If-Range, xử lý 416, mất mạng giữa chừng, concurrent download, fsync/atomic rename và checksum. Viết decision table cho status 200/206/416/503 và thay đổi ETag. Không làm mất file hoàn chỉnh cũ nếu download mới thất bại.
