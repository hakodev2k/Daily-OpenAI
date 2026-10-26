$ErrorActionPreference = "Stop"
# Xóa riêng container/volume của lab này; không dùng trên dữ liệu production.
docker compose -f "$PSScriptRoot/compose.yaml" down -v
exit $LASTEXITCODE
