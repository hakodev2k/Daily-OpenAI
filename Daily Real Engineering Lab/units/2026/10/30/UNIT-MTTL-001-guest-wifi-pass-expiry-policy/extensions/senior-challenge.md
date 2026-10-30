# Senior extension — Requirement change

Sau rollout, khách có thể duy trì phiên Wi-Fi 6 giờ và không kiểm tra lại quyền tại mỗi request. Business yêu cầu thu hồi có hiệu lực trong 5 giây cả với phiên đang mở.

Hãy sửa ADR: phân biệt admission check và session enforcement, đề xuất gateway/session revalidation hoặc push revocation, nêu độ tin cậy khi mất kết nối, clock skew, retry storm và tiêu chí đo SLA. Không mặc định signed token hoặc cache sẽ tự giải quyết yêu cầu.
