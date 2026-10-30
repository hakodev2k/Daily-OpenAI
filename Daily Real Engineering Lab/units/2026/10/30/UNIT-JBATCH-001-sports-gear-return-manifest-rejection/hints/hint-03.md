# Hint 3 — Concept

Một stream có thể cung cấp nhiều record hợp lệ trước khi phát sinh JsonException. Cân nhắc ranh giới staging, validation và atomic publication. ManifestStore có primitive để áp dụng cả lô như một snapshot; cần tính đến memory bound và transaction thực tế.
