# After
Chạy \`./verify.ps1\`: exit code 0 khi JSON hợp lệ trả 200 và \`accepted:V-104\`, malformed JSON vẫn trả 400, audit đủ 2 fingerprint không rỗng và khác nhau. \`./reproduce.ps1\` có thể trả nonzero sau khi đã sửa vì triệu chứng ban đầu không còn; đây là hành vi đúng.
