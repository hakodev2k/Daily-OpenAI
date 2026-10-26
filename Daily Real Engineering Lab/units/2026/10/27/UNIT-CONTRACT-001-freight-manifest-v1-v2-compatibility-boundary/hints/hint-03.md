# Hint 03
Một phương án: dispatch tường minh theo `schemaVersion` (v1 legacy không khai báo), validate mỗi version độc lập, reject unknown/ambiguous và không default missing hazard thành `NONE`.
