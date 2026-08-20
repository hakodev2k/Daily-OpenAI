# Day-002 Blueprint — Nghe trước, transcript sau

## 1. Lesson Identity

| Field | Planned Value |
|---|---|
| Lesson | `Day-002` |
| Title | Nghe trước, transcript sau |
| Phase | Phase 0 — English Orientation (`P00`) |
| Module | `P00-M01` — Học cách học và dùng hệ thống |
| Level Orientation | Zero → Pre-A1 |
| Lesson Type | Foundation / Listening routine |
| Dominant Mode | Learn Mode |
| TOEIC Orientation | Chưa tích hợp TOEIC; chỉ xây prerequisite cho Listening về sau |
| Operational Source | `COURSE_STATE.md`: `Next Lesson: Day-002` |
| Remediation Override | Không có; learner performance chưa được đánh giá |
| Planned Output of Next Stage | Một lesson hoàn chỉnh cho `Day-002`; blueprint này không phải final lesson |

## 2. Planning Decision and Scope Lock

Blueprint này lập kế hoạch cho đúng một lesson: **Day-002 — Nghe trước, transcript sau**.

Primary Goal ràng buộc:

> Người học thực hiện đúng quy trình **first listen → replay có mục tiêu → trả lời → transcript reveal → nghe lại**.

Lesson Generator không được mở rộng bài thành:

- bài pronunciation hoặc phonics;
- bài kiểm tra khả năng nghe được bao nhiêu English words;
- bài dictation hoàn chỉnh;
- bài vocabulary ngoài các nhãn thao tác đã được duyệt;
- bài TOEIC Part 1–4;
- bài dạy bảng chữ cái, sound system, IPA, syllable, final sound hoặc connected speech;
- bài đánh giá level, CEFR hoặc TOEIC score;
- bài cập nhật learner state hay `COURSE_STATE.md`.

## 3. Source-of-Truth Audit

Blueprint phải được thực thi theo các quyết định đã xác nhận từ repository:

- `COURSE_SPEC.md`: người học zero/Pre-A1; Vietnamese-led; transcript chỉ reveal sau honest attempt; không có artificial length limit; không invent learner data.
- `ROADMAP.md`: Phase 0 ưu tiên learning conventions, audio/transcript routine và sound awareness; chưa có TOEIC pressure.
- `LEARNING_SYSTEM.md`: default Learn Mode listening protocol bắt đầu bằng purpose, first listen không transcript, targeted replay, response, transcript reveal, sound–text comparison và later transcript-free re-listen.
- `KNOWLEDGE_DEPENDENCIES.md`: audio navigation + attention routine là prerequisite trước sound discrimination và spoken-word recognition.
- `CURRICULUM_MASTER.md`: P00-M01 xây cách dùng hệ thống; Day-002 thuộc foundation/listening routine.
- `curriculum/phase-00-english-orientation.md`: Day-002 chỉ dạy protocol và audio–transcript distinction; không dạy pronunciation lesson hay đếm từ nghe được.
- `COURSE_STATE.md`: Day-001 đã accepted; Day-002 là next authorized lesson; không có learner remediation evidence.
- State files: grammar chưa được giới thiệu; learner performance chưa được đánh giá; immediate review phải lấy Day-001 navigation, attempt-before-feedback, `listen` cue và access-vs-performance distinction.
- `lessons/phase-00/Day-001.md`: actual delivered lesson đã dạy four lesson regions, five-step cycle, Learn Mode, attempt-before-feedback, `listen/read/choose/check`, và technical access khác language performance.
- `reviews/Day-001-review.md`: hai deferred minor content issues không phải learner errors. Day-002 cần tránh response options chồng lấn và tránh unnecessary English meta-language.

Không có `specs/` files trong repository tại thời điểm planning; không được giả định các standard chưa tồn tại.

## 4. Why This Lesson Exists

Mọi listening lesson sau này cần evidence thật về điều người học nhận ra từ âm thanh. Nếu transcript xuất hiện trước attempt, learner có thể hiểu nhờ chữ nhưng tưởng rằng mình đã nghe được. Ngược lại, cấm transcript hoàn toàn sẽ làm mất công cụ quan trọng để nối sound với written form sau attempt.

Day-002 vì vậy xây một routine an toàn:

1. nghe trước khi nhìn chữ;
2. lưu một dấu vết first attempt rất đơn giản;
3. replay với một mục tiêu cụ thể;
4. trả lời trước reveal;
5. dùng transcript để kiểm tra và học;
6. nghe lại không phụ thuộc vĩnh viễn vào transcript.

Lesson không nhằm chứng minh người học “nghe tốt”. Nó nhằm chứng minh người học **dùng listening material đúng quy trình**.

## 5. Lesson Central Question

> **Tại sao tôi cần nghe và tự trả lời trước khi xem phần chữ của audio?**

Final lesson phải duy trì câu hỏi này xuyên suốt và quay lại nó trong self-explanation, mastery check và one-minute summary.

## 6. New Knowledge Budget

### New Core Knowledge — phải học ngay

1. Trình tự listening protocol:
   `first listen → record first attempt → purposeful replay → respond → reveal transcript → compare → re-listen`.
2. Transcript là bản chữ đại diện cho nội dung audio; transcript không phải chính âm thanh.
3. First-attempt evidence phải được ghi trước khi transcript thay đổi điều learner biết.
4. Replay có mục tiêu: mỗi lần nghe lại cần biết mình đang tìm signal, difference hoặc information nào.

### Supporting Knowledge — chỉ đủ để vận hành core

- Cách ghi một trạng thái đơn giản: `nghe thấy`, `chưa chắc`, `chưa nhận ra`.
- Cách phân biệt asset/control problem với difficulty nhận ra English.
- Cách dùng play, pause/stop và replay khi asset tồn tại.
- Cách để transcript trong vùng reveal tách biệt.

### Review Knowledge — đã xuất hiện ở Day-001

- Tìm instruction, activity/input, response area và feedback.
- Attempt/respond trước khi mở feedback/answer.
- Nhận ra và thực hiện cue `listen`.
- Technical audio access khác language-listening performance.
- Learn Mode cho phép feedback và retry sau attempt.

### Useful Extension — hữu ích nhưng không phải mastery target

- Confidence label đơn giản sau first listen.
- Tự nói mục tiêu replay trước khi bấm phát lại.
- Một audio access record tách riêng với language observation.

### Future Preview — chỉ nhắc ngắn

- Sound–text mapping.
- Dictation.
- Shadowing.
- Listening for gist/detail.
- TOEIC Listening Parts 1–4.

Generator phải nói rõ các mục Future Preview chưa cần học hoặc thực hành đầy đủ hôm nay.

## 7. Prerequisite Audit

| Prerequisite | Status | Evidence / Planning Action |
|---|---|---|
| Mở và đọc Markdown lesson | Available in accepted Day-001 content; learner performance not evaluated | Bắt đầu bằng access check ngắn; không tuyên bố mastered |
| Tìm instruction, activity/input, response và feedback | Introduced and practiced in Day-001 | Retrieve briefly in changed listening layout; không reteach toàn bộ Day-001 |
| Attempt trước feedback | Introduced and practiced in Day-001 | Dùng làm prerequisite retrieval bắt buộc trước transcript protocol |
| Nhận ra cue `listen` | Introduced with Vietnamese/icon support | Retrieve với support giảm nhẹ; vẫn gloss bằng tiếng Việt |
| Phân biệt technical access và English listening difficulty | Introduced in Day-001 | Reconfirm bằng status record rõ ràng, mutually exclusive ở access layer |
| Play/pause-or-stop/replay controls | Operationally introduced conditionally; asset may not exist | Kiểm tra tại chỗ; nếu unavailable thì mark technical blocker, không infer learner weakness |
| English vocabulary comprehension | Not established | Không được dùng làm hidden prerequisite; audio input phải tối giản và taught/supported |
| Pronunciation concepts | Not taught | Không yêu cầu hoặc dùng terminology về sounds/IPA/stress |
| Grammar | Not taught | Không dùng grammar làm target hoặc explanation dependency |

### Prerequisite Decision

Không có evidence về remediation requirement. Proceed với Day-002 nhưng final lesson phải có một quick operational retrieval. Nếu actual learner không thể vận hành audio hoặc reveal sequence, lesson phải chỉ ra navigation/access remediation branch thay vì tiếp tục giả vờ có valid listening evidence.

## 8. Terminology Audit

| Term | Classification | Learner-facing treatment |
|---|---|---|
| `listen` | Already introduced; review target | Giữ English label, gloss `nghe`, có icon nếu hữu ích |
| `pause` | Must be explained now as support/target label | Gloss `tạm dừng`; không phân tích word class |
| `replay` | Must be explained now as target functional label | Gloss `phát/nghe lại` |
| `transcript` | Must be explained now as core functional term | Dùng Vietnamese-first: “bản chữ của audio (`transcript`)” |
| `audio` | Support course label | Giải thích là phần âm thanh/tệp âm thanh; không bắt memorization nếu không cần |
| `first listen` | Core procedure label | Vietnamese-first: “lượt nghe đầu tiên (`first listen`)” |
| `reveal` | Avoid as unexplained learner requirement | Viết “mở/xem transcript” trước, English chỉ giữ nếu UI dùng label đó |
| `signal` | Avoid or explain locally | Ưu tiên “âm hiệu/ngữ liệu cực ngắn” |
| `confidence` | Optional | Viết “mức chắc chắn”; không bắt học English word |
| `sound–text mapping` | Future terminology | Chỉ preview bằng Vietnamese; không biến thành prerequisite |
| `dictation`, `shadowing` | Future terminology | Nếu nhắc, gloss ngắn và nói chưa học hôm nay |
| phoneme, syllable, stress, linking, IPA | Future / avoid now | Không dùng trong learner-facing core instruction |

Do review Day-001 đã phát hiện hidden meta-vocabulary, generator phải ưu tiên Vietnamese equivalents cho `level`, `item`, `passage`, `audio control`, `learning cycle`, `highlight`, `homework` và các thuật ngữ vận hành tương tự nếu chúng xuất hiện.

## 9. Learning Objectives

### Knowledge

Sau lesson, learner có thể nói bằng tiếng Việt:

- transcript là gì và transcript khác audio như thế nào;
- vì sao first listen cần diễn ra trước transcript reveal;
- vì sao replay nên có mục tiêu.

### Recognition

Learner có thể nhận ra các cue `listen`, `pause`, `replay`, `transcript` khi có Vietnamese support phù hợp.

### Process / Independent Operation

Learner có thể thực hiện đúng protocol trên hai rehearsal khác nhau:

1. first listen không transcript;
2. ghi first-attempt observation;
3. replay với mục tiêu được nêu;
4. trả lời;
5. mở transcript;
6. so sánh;
7. nghe lại.

### Listening

Learner có thể xử lý hai hoặc ba audio signals/learned labels cực ngắn ở tốc độ very slow and clear mà không bị yêu cầu đạt vocabulary-comprehension score.

### Reading

Learner có thể dùng transcript sau attempt để kiểm tra “audio đã nói gì” ở mức label hoặc signal cực ngắn.

### Self-Monitoring

Learner có thể ghi nhận riêng:

- audio access/control status;
- first-attempt language observation;
- transcript/replay step used.

### TOEIC Application

Không có objective làm TOEIC item. Learner chỉ cần biết routine này sẽ bảo vệ evidence và hỗ trợ Parts 1–4 trong tương lai.

## 10. Success Criteria

Final lesson phải tạo observable evidence sau:

- Sắp xếp đúng các bước protocol trong một changed-order task.
- Hoàn thành hai protocol rehearsals mà không mở transcript trước response.
- Nêu đúng ít nhất một reason vì sao transcript xuất hiện sau attempt.
- Phân biệt audio và transcript trong các scenario đơn giản.
- Nêu một mục tiêu trước replay thay vì replay vô định.
- Ghi first-attempt observation trước reveal.
- Tách technical status khỏi “tôi chưa hiểu/nhận ra English”.
- Nghe lại sau transcript ít nhất một lần khi asset hoạt động.

Các tiêu chí là future evidence requirements, không phải learner result. Không được invent percentage hoặc claim pass.

## 11. Knowledge Map

```text
Day-001 lesson navigation
        +
attempt before feedback
        +
technical access ≠ language performance
        ↓
first listen before transcript
        ↓
purposeful replay + first-attempt record
        ↓
transcript comparison + re-listen
        ↓
future sound–text mapping and all listening lessons
```

## 12. Core Mental Model

Final lesson nên dùng một flow nhỏ, nhất quán:

```text
NGHE 1 → GHI ĐIỀU NGHE ĐƯỢC → NGHE LẠI CÓ MỤC TIÊU → TRẢ LỜI
                                                         ↓
NGHE LẠI KHÔNG NHÌN CHỮ ← SO SÁNH AUDIO–CHỮ ← MỞ TRANSCRIPT
```

Mental model phải truyền đạt hai ý:

1. transcript là công cụ học **sau** attempt, không phải vật cấm;
2. cuối protocol cần quay lại audio để tránh chỉ học bằng mắt.

Không cần decision tree phức tạp hoặc diagram trang trí.

## 13. Concept Teaching Order

Generator phải dạy theo progression sau, có thể đặt tên section khác nếu chức năng giữ nguyên:

1. **Safety and purpose:** Không cần nghe giỏi; hôm nay học cách dùng audio và transcript.
2. **R1 retrieval from Day-001:** Tìm instruction/response/feedback, nhận `listen`, attempt trước reveal.
3. **Concrete analogy:** Nghe một tiếng chuông rồi mới nhìn nhãn giúp biết tai đã nhận gì; analogy không được thay thế English audio practice.
4. **Audio vs transcript:** Nêu relation và difference bằng Vietnamese-first explanation.
5. **Why first listen matters:** first-attempt evidence bị mất nếu learner nhìn chữ trước.
6. **Protocol overview:** giới thiệu từng bước và chức năng của bước.
7. **Worked demonstration:** model bằng neutral/nonlinguistic signal hoặc ultra-short supported label; không kiểm tra English level.
8. **Guided rehearsal:** learner làm với prompts rõ, transcript hidden.
9. **Purposeful replay:** learner xác định mục tiêu replay.
10. **Transcript reveal and comparison:** reveal sau response; mark match/mismatch không phán xét.
11. **Re-listen:** nghe lại với transcript đã đóng/che nếu feasible.
12. **Second rehearsal with faded hints:** new signal/label, layout variation.
13. **Common mistakes and correction:** transcript-first, evidence-late, replay-without-purpose, technical/language confusion.
14. **Independent mini-protocol and conceptual check.**
15. **Mastery interpretation, remediation route, summary, future review and Day-003 connection.**

Không dạy formal pronunciation rule trước intuitive process.

## 14. Required Explanation Angles

Final lesson phải giải thích đầy đủ nhưng beginner-friendly:

- Audio là gì trong course.
- Transcript là gì.
- Audio và transcript liên quan nhưng không giống nhau.
- First listen nhằm thu evidence gì.
- Vì sao learner cần ghi một response/observation dù chưa chắc.
- Replay có mục tiêu nghĩa là gì.
- Khi nào transcript được mở trong Learn Mode.
- Transcript dùng để làm gì sau reveal.
- Vì sao cần re-listen sau khi so sánh chữ.
- Technical failure khác language difficulty như thế nào.
- Vì sao chưa nghe được label không phải thất bại.
- Learn Mode cho phép replay/retry nhưng vẫn cần honest first attempt.
- TOEIC relevance chỉ ở mức foundation, không có test item.

Không cần giải thích:

- speech sounds classification;
- IPA;
- accent correctness;
- connected speech rules;
- listening subskills như gist/inference ở depth chính thức;
- test replay policy của TOEIC.

## 15. Intuitive Explanation Requirement

Formal-looking protocol không được xuất hiện trước learner-friendly intuition.

Generator nên truyền đạt ý tương đương:

> Nếu nhìn bản chữ trước, mắt đã cho bạn câu trả lời. Khi đó bạn khó biết tai mình đã nhận ra điều gì. Vì vậy, hãy nghe và ghi lại một dấu vết ngắn trước; sau đó transcript giúp bạn kiểm tra và học, rồi bạn quay lại nghe lần nữa.

Không dùng claims tuyệt đối như “never use transcripts” hoặc “nghe nhiều lần luôn tốt”. Transcript là scaffold đúng thời điểm; replay phải phục vụ mục tiêu.

## 16. Example Progression

### Category A — Nonlinguistic access example

- Một âm hiệu rõ dùng để kiểm tra play/pause/replay và process order.
- Không tính là English Listening evidence.
- Chỉ dùng nếu cần tách technical access khỏi language task.

### Category B — Previously introduced label

- Một label cực ngắn đã được Day-001 giới thiệu, ưu tiên `listen`.
- Very slow and clear; one speaker.
- Transcript chỉ chứa chính label, không thêm sentence.

### Category C — New supported functional label

- Một trong `pause` hoặc `replay`, được pre-taught bằng Vietnamese/icon trước audio.
- Không yêu cầu learner đoán nghĩa từ âm thanh.

### Category D — Contrast/trap scenario

- Process scenarios: learner nhìn transcript trước, replay không goal, hoặc technical audio unavailable.
- Đây là scenario analysis, không phải listening comprehension trap.

Progression phải đi từ full modeling → guided → reduced prompt → independent process. Không cần workplace hoặc TOEIC-style examples ở Day-002.

## 17. Example Analysis Requirements

Chỉ một số examples cần breakdown sâu. Với protocol demonstration, analysis nên chỉ ra:

- instruction nằm ở đâu;
- transcript đang đóng hay mở;
- learner cần ghi gì sau first listen;
- replay target là gì;
- response được lưu trước reveal hay chưa;
- transcript có gì;
- first attempt match/mismatch transcript ở điểm nào;
- re-listen thay đổi recognition hay chỉ giúp learner biết answer bằng mắt;
- access status và language observation là hai records khác nhau.

Không phân tích subject, verb, word class, phoneme hoặc IPA.

## 18. Vocabulary Plan

### Vocabulary Theme

Listening controls and transcript routine.

### Target Vocabulary

| Label | Function | Intended Depth Today |
|---|---|---|
| `listen` | nghe | Review; recognize and act with reduced support |
| `pause` | tạm dừng | Recognize with Vietnamese/icon support |
| `replay` | phát/nghe lại | Recognize and use in protocol |
| `transcript` | bản chữ của audio | Understand core function and timing |

### Support Vocabulary

- audio / phần âm thanh;
- first listen / lượt nghe đầu;
- response / câu trả lời hoặc ghi nhận;
- play/stop if interface requires them;
- instruction, activity/input, response area, feedback as Day-001 region labels.

Support labels không tự động trở thành memorization target.

### Review Vocabulary

- `listen` là high-priority retrieval.
- `read`, `choose`, `check` chỉ reuse tự nhiên nếu layout cần; không bắt ôn cả bốn như một quiz giống Day-001.

### Incidental Vocabulary

Giữ gần bằng không. Nếu audio dùng English label, learner phải đã được gloss và thấy function trước. Không thêm greetings, names, numbers, object nouns hoặc sentence vocabulary.

### Collocations / Word Families / Paraphrases

Không required. Chỉ functional mappings:

- `replay` ↔ “nghe lại/phát lại” trong interface;
- `transcript` ↔ “bản chữ của phần âm thanh”.

Không dạy word families, synonyms hoặc grammar behavior.

### Potential Confusions

- `listen` vs `read`;
- `pause` vs `stop` nếu UI có cả hai;
- `replay` vs mở transcript;
- audio file vs transcript text;
- “nghe thấy có âm thanh” vs “hiểu/nhận ra English”.

## 19. Pronunciation Plan

**Pronunciation Focus: Primary awareness only; no pronunciation lesson.**

### Target Awareness

- Spoken form và written label là hai representations liên quan.
- Learner có thể chưa match chúng ở first listen; đây là expected learning evidence.

### Vietnamese Learner Risk

Không đưa ra sound-specific stereotype hoặc giả định cá nhân. Chỉ nêu general risk phù hợp: người mới có thể phụ thuộc vào chữ hoặc kỳ vọng spelling và sound phải giống hệt từng ký tự.

### Recognition Objective

Sau transcript reveal và re-listen, learner nhận ra rằng written label corresponds to the short audio token.

### Production Objective

Không bắt buộc. Optional repetition chỉ được dùng như một cách chú ý sound, không đánh giá accent hoặc correctness.

### Connected Speech

Not required. Không dùng sentence, reduction, linking hoặc natural conversational rate.

## 20. Listening Plan

### Listening Objective

Thực hiện protocol đúng và tạo clean first-attempt evidence, không đo broad listening ability.

### Input Difficulty

- Ultra-short tokens/signals.
- Known or explicitly supported functional labels.
- Không có unknown sentence grammar.

### Audio Difficulty

- **Very slow and clear.**
- One speaker.
- Neutral, internationally intelligible delivery.
- No intentional reductions, noise, accent challenge or distractor.

### Context

Course-interface/listening routine only.

### Target Information

- Với nonlinguistic signal: sound occurred / control worked.
- Với label: which pre-taught label was heard, or whether learner is uncertain.

### Required Listening Stages

1. Purpose preview without answer.
2. Technical access status check if needed.
3. First listen without transcript.
4. Immediate observation/confidence record.
5. Target for replay stated.
6. One controlled replay; additional replay only if generator explains why.
7. Response recorded.
8. Transcript reveal.
9. Brief comparison.
10. Re-listen, ideally after hiding/closing transcript.
11. Process reflection.

### Likely Listening Traps

Không dùng language distractors. Relevant process traps:

- learner opens transcript before first listen;
- learner records evidence only after seeing transcript;
- learner replays repeatedly without a target;
- learner reports “I am bad at listening” when asset/control failed;
- learner reads transcript and skips final re-listen.

### Transcript Usage Policy

- Transcript must be physically separated using a collapsed block or clearly later section when platform allows.
- Instruction immediately before audio must say not to open transcript yet.
- Transcript may appear only after defined response.
- Transcript must match final audio exactly.
- If audio does not exist, generator must label it as an asset requirement, not pretend playback occurred.
- The transcript must not expose answers to subsequent independent activity.

### Post-Listening Analysis

Keep analysis procedural and minimal: what was heard, what text shows, whether access worked, and what learner noticed on re-listen. Full sound analysis is deferred.

## 21. Required Audio Assets

Lesson Generator may specify assets; it must not claim they already exist.

Minimum useful asset set:

1. **Access signal** (optional if platform playback already verified): short nonlinguistic tone.
2. **Rehearsal token A:** previously taught label such as `listen`.
3. **Rehearsal token B:** newly supported label such as `replay` or `pause`.

For every requested asset specify:

- stable placeholder ID/path;
- exact canonical transcript where language exists;
- one speaker;
- very slow and clear delivery;
- no background music/noise;
- short duration;
- no extra spoken introduction or answer cue;
- transcript reveal location;
- QA requirement: waveform–transcript agreement and usable play/pause/replay controls.

Do not create audio binary during lesson planning.

## 22. Reading Plan

### Reading Objective

Use a one-word or ultra-short transcript after listening to verify what the audio contained.

### Text Type

- Interface labels.
- One-token transcript.
- Short Vietnamese protocol instructions.

### Complexity

Very Beginner. No English passage or sentence comprehension.

### Reading Strategy

Compare the written token with the sound after attempt; learner may point to same/different/uncertain rather than explain in English.

### Question Types

- Identify which region is transcript.
- Determine whether transcript should be open at the current step.
- Match heard supported label to written option after proper attempt.

### Evidence and Inference

All answers must be explicit. No inference, skimming, scanning or paraphrase requirement.

## 23. TOEIC Application Plan

**Level: TOEIC awareness only; no TOEIC integration.**

Final lesson may include one short note:

- future TOEIC Parts 1–4 use audio without transcript during scored attempts;
- Day-002 routine builds listening independence and later review habits.

Must not include:

- TOEIC-style question;
- part mechanics;
- timed activity;
- score estimate;
- replay rules claimed as full TOEIC policy;
- distractor training.

## 24. Paraphrasing Plan

**Not required as a language-learning target.**

Allowed functional equivalences:

- `replay` = phát lại/nghe lại within the course interface;
- `transcript` = bản chữ của audio.

Do not introduce synonyms, word-family equivalence or TOEIC paraphrasing.

## 25. Required Comparisons

Final lesson must compare:

1. **Audio vs transcript** — sound input versus written representation.
2. **First listen vs post-transcript re-listen** — evidence before support versus learning after support.
3. **Purposeful replay vs repeated replay without focus**.
4. **Technical access status vs language observation**.
5. **Transcript as support vs transcript-first dependence**.

Comparison is only valid if both sides are explained in plain Vietnamese. Avoid absolute claim that transcript is “bad”.

## 26. Common Mistakes and Preventive Teaching

### Process Mistakes

- Mở transcript ngay khi thấy link/details.
- Không ghi first attempt trước reveal.
- Ghi lại first attempt từ memory sau khi đã nhìn transcript.
- Replay quá nhiều lần mà không nêu target.
- Sau khi nhìn transcript, không quay lại nghe.
- Treat one successful replay as durable listening mastery.

### Access Mistakes

- Mute/volume/file unavailable nhưng learner tự kết luận mình nghe kém.
- Control exists but playback fails; response status không distinguish được failure type.
- Chọn nhiều status chồng lấn vì options không mutually exclusive.

### Vocabulary/Terminology Mistakes

- Nhầm `replay` với mở transcript.
- Nhầm transcript là audio.
- Hidden English meta-language làm learner không hiểu instruction.

### Prevention Requirements

- Tách access status thành exactly one mutually exclusive choice:
  1. verified play–pause/stop–replay;
  2. asset/control unavailable;
  3. control available but technical playback failed.
- Tách language observation thành optional separate field: recognized / uncertain / not recognized.
- Không dùng access result làm Listening score.

## 27. Vietnamese Learner Considerations

- Vietnamese-led instructions are mandatory.
- Không giả định learner quen học listening bằng transcript.
- Giải thích rằng English spelling không phải “audio được viết ra theo từng âm” nhưng không dạy spelling–sound system hôm nay.
- Không yêu cầu pronunciation imitation hoặc accent correction.
- Không diễn giải “không nhận ra token” thành lỗi đặc trưng của mọi Vietnamese learner.
- English interface labels phải luôn có Vietnamese meaning/icon tại first required use.
- Không dùng shame-based language khi learner nghe chưa được.

## 28. Exercise Architecture

### Block A — Immediate Retrieval from Day-001

- **Purpose:** verify operational prerequisite without reteaching whole lesson.
- **Tasks:** locate instruction/response/feedback; choose correct attempt-before-reveal order; recognize `listen`; classify technical vs language situation.
- **Difficulty:** Very Beginner.
- **Support:** Vietnamese and familiar layout, then one changed layout.
- **Feedback:** immediate after attempt; explain function and order.

### Block B — Recognition of New Routine

- **Purpose:** recognize audio, transcript and protocol steps.
- **Tasks:** match labels/functions; sort audio vs written representation; order steps.
- **Support:** full model, icon and Vietnamese gloss.
- **Hints:** allowed; no language answer hidden in hint.

### Block C — Worked Protocol

- **Purpose:** observe one complete valid run.
- **Input:** nonlinguistic or previously taught label.
- **Support:** numbered steps and explicit closed transcript.
- **Learner action:** still must record one observation; not passive demonstration only.

### Block D — Guided Rehearsal

- **Purpose:** execute protocol with one ultra-short supported label.
- **Support:** checklist per step; target replay prompt.
- **Feedback:** after transcript reveal; require re-listen.

### Block E — Error Correction

- **Purpose:** diagnose process errors.
- **Tasks:** find wrong step, explain consequence, repair sequence.
- **Scenarios:** transcript-first; response-after-reveal; replay-without-goal; access/language confusion.

### Block F — Second Rehearsal with Faded Support

- **Purpose:** transfer routine to a new token/layout.
- **Support:** Vietnamese instruction and standard labels, fewer numbered prompts.
- **Hints:** none that reveal token or sequence during attempt.

### Block G — Independent Mini-Protocol

- **Purpose:** test operation without step-by-step prompting.
- **Skills:** first attempt, evidence record, purposeful replay, response, reveal, compare, re-listen.
- **Input:** one known/supported token different from worked example.
- **Feedback:** complete process feedback after submission.

### Block H — Conceptual Check and Self-Explanation

- **Purpose:** verify learner understands why, not only memorizes order.
- **Response:** Vietnamese oral/written/selection allowed.

No block should require grammar, sentence production or unknown vocabulary.

## 29. Exercise Depth and Volume Guidance

Do not set an arbitrary total. Minimum pedagogical coverage requires:

- at least one prerequisite retrieval;
- at least one worked protocol;
- at least two learner-executed rehearsals using different tokens/layouts;
- at least one purposeful-replay decision;
- process error correction covering all four major failure patterns;
- one independent mini-protocol;
- one conceptual explanation of transcript timing;
- one clean access-vs-language record.

Add items only when they expose a different dimension. Do not repeat many one-word audios merely to increase quantity.

## 30. Guided Practice and Hint Fading

Suggested scaffold fade:

```text
Vietnamese + icon + numbered protocol + visible purpose
→ Vietnamese + icon + short checklist
→ Vietnamese instruction + standard labels
→ independent protocol card with no answer-revealing step prompts
```

Transcript remains hidden at the correct stage even during guided work. “Guided” does not mean transcript-first.

## 31. Independent Practice Requirement

Independent practice must test process, not English vocabulary breadth.

Learner must independently:

- start with transcript closed;
- listen once;
- record an observation before support;
- select or state a replay target;
- replay;
- commit a response;
- reveal transcript;
- compare;
- close/cover transcript and listen again where technically possible.

The generator must not tell the token in the heading, filename or nearby text before the attempt.

## 32. Mixed Practice

Only local interleaving is appropriate:

- Day-001 region/navigation routine;
- Day-001 attempt-before-feedback;
- `listen` label;
- Day-002 audio/transcript/replay routine.

No grammar, pronunciation concepts, alphabet knowledge or TOEIC format may enter the mix.

## 33. Error-Correction Practice

For each process scenario, learner should:

1. identify the first invalid step;
2. say why evidence/learning is weakened;
3. move or replace the step;
4. state the corrected next action.

Why-wrong analysis is mandatory for plausible alternatives. Example categories may be specified, but generator must create original final scenarios.

## 34. Production and Translation Policy

### Production

- Vietnamese self-explanation.
- Simple checkbox/ordering/short status labels.
- Optional repetition of one label after transcript reveal.
- No English sentence creation.

### Translation

- English labels receive contextual Vietnamese gloss.
- English → Vietnamese is allowed for interface function.
- Vietnamese → English production is not required.
- Avoid permanent word-by-word listening strategy; transcript is for comparison after attempt.

## 35. Real-World Application

Appropriate application:

- Using an audio lesson, pronunciation dictionary example or future workplace recording responsibly: listen first, note, then inspect text if available, and re-listen.

Keep this as a short transfer note. Do not add new real-world vocabulary or dialogue.

## 36. Mini-Conversation Plan

**Not required.**

A dialogue would add speaker roles, grammar and vocabulary not needed for the protocol. Use isolated supported tokens/signals only.

## 37. Diagnostic Section

Include a quick operational diagnostic at the beginning:

- Can learner locate the `listen` cue?
- Is an audio asset/control available?
- Can learner keep feedback/transcript closed until after response?
- Can learner record a simple answer?

This diagnostic must not test English level. If audio is unavailable, provide a documented technical branch and continue only with process simulation where valid; do not fabricate completed listening evidence.

## 38. Retrieval Practice

### Immediate Review (R1 from Day-001)

- Locate instruction, activity/input, response and feedback.
- Attempt/respond before reveal.
- Recognize and act on `listen`.
- Classify technical access vs language difficulty.

### Short-Term Review

- Within Day-002 (R0): reorder protocol after an intervening activity.
- End of lesson: explain why transcript is delayed.

### Older Retrieval

None beyond Day-001.

### Error-Based Review

No confirmed learner error data. Do not pre-populate transcript-first dependence or navigation difficulty as learner patterns.

## 39. Spaced Repetition Plan

After actual lesson completion, State Updater may schedule based on evidence:

- **Next lesson (Day-003):** attempt → feedback/retry plus transcript timing if audio activity appears.
- **Around Day-005:** retrieve listening protocol while teaching retrieval routines.
- **Day-006 module checkpoint:** independently operate lesson/audio/feedback/review cycle.
- **Day-007 onward:** embed first-listen-before-transcript in every relevant listening activity.
- **~3 lessons later:** retrieve core sequence using a changed layout/token.
- **~1 week later:** sample transcript discipline and purposeful replay in a new listening context.
- **Later cumulative review:** maintain as embedded behavior rather than repeatedly quizzing the same sequence if stable.

Schedule categories:

- Core listening protocol.
- Target labels `replay` and `transcript`.
- Access-vs-language distinction.
- Common process mistake only if actually observed.

Blueprint does not update queue/state.

## 40. Concept Extension and “What If?” Plan

### Core

- Correct protocol order.
- Audio/transcript distinction.
- Purposeful replay.
- First-attempt evidence.

### Useful Extension

- Confidence label.
- One additional optional re-listen after a short break.

### Future Preview

- Sound–text mapping, dictation, shadowing and future TOEIC Listening.

### What If Transformations

Final lesson should explore:

- What if transcript was opened before first listen?
- What if learner heard sound but did not recognize the word?
- What if audio did not play?
- What if learner wants to replay five times without a target?
- What if response changes after transcript reveal—how should first attempt be preserved?
- What if learner can read the label but still cannot recognize it in audio?
- What if learner recognizes it after transcript and re-listen—what has been learned and what is not yet proven durable?

Do not turn future concepts into assessed core.

## 41. Conceptual Questions

Required question categories:

1. Why do we listen before opening transcript?
2. What information does the first-attempt record preserve?
3. How is audio different from transcript?
4. What makes a replay purposeful?
5. Why do we listen again after seeing transcript?
6. If audio does not play, what should be checked before judging Listening ability?
7. Does recognizing a label after transcript prove long-term mastery? Why or why not?

Allow Vietnamese response or structured choice. Do not require English production.

## 42. Self-Explanation Requirement

Learner must complete at least one:

- Explain the protocol in simple Vietnamese.
- Explain why transcript is useful but should not come first.
- Explain the difference between “audio did not work” and “audio worked but I did not recognize the label.”

Evaluate concept accuracy, not writing quality.

## 43. Quiz Plan

### Purpose

Check readiness to use the listening protocol in future lessons.

### Coverage

- protocol order;
- audio vs transcript;
- purposeful replay;
- first-attempt evidence;
- technical vs language status;
- target functional labels;
- one independent mini-protocol observation.

### Question Diversity

- step ordering;
- scenario choice;
- label/function matching;
- identify first wrong step;
- one why question;
- one unprompted protocol execution.

### Difficulty

Very Beginner; Vietnamese-accessible; no unknown language.

### Mixed Review

Include attempt-before-feedback and `listen` from Day-001.

### TOEIC

None.

Quiz must use new layout/scenarios, not copy guided rehearsal. No numeric threshold alone determines mastery.

## 44. Answer Explanation Requirements

Every checked/scored task must eventually explain, when relevant:

- correct step/order/status;
- why it protects first-attempt evidence;
- clue showing transcript must remain closed;
- difference between access and comprehension;
- why each plausible alternative is wrong;
- corrected next action;
- whether retry or technical remediation is appropriate.

Simple label matching can have concise feedback. Process-order and access classification require deeper feedback.

Do not expose transcript, token or answer before the declared attempt.

## 45. Why-Wrong and Ambiguity Prevention

Detailed why-wrong analysis is mandatory when learner:

- opens transcript before responding;
- treats transcript as audio;
- records only post-transcript answer;
- replays without a goal;
- skips final re-listen;
- treats unavailable audio as weak English;
- selects an overlapping access status.

Item rules:

- exactly one defensible keyed answer for scored choices;
- enough sequence/context to determine the answer;
- no answer leaked by heading, filename, transcript summary or option formatting;
- access statuses must be mutually exclusive;
- language observation must not be mixed into access status;
- avoid obscure UI assumptions; give fallback when `<details>` or audio controls are unsupported.

## 46. Optional Challenge

Appropriate low-stakes challenge:

- Given a deliberately scrambled listening card, redesign the order or point out every protocol violation.

Challenge may remove icons or numbered prompts but must retain Vietnamese accessibility and known labels. It cannot introduce a longer sentence, natural-speed audio or dictation. It is not required to pass.

## 47. Mastery Check

### Dimensions

1. **Protocol knowledge:** knows and explains sequence.
2. **Independent operation:** executes a new mini-protocol without transcript-first behavior.
3. **Representation distinction:** audio versus transcript.
4. **Replay control:** states a replay target.
5. **Evidence integrity:** preserves first attempt.
6. **Access classification:** separates technical and language outcomes.
7. **Label recognition:** uses `listen`, `pause`, `replay`, `transcript` with phase-appropriate support.

### Strong Mastery

- Executes two different rehearsals correctly with transcript closed until response.
- Explains transcript timing and audio/transcript distinction accurately.
- Uses purposeful replay and final re-listen.
- Creates a clean first-attempt record.
- Distinguishes technical failure from language difficulty.

**Next action:** Proceed to Day-003; schedule routine retrieval.

### Partial Mastery

- Understands core reason but needs one process cue, confuses one label, forgets to state replay goal, or needs support to preserve first attempt.
- Does not repeatedly reveal transcript first after correction.

**Next action:** Day-003 may proceed with a short listening-protocol refresher if routine remains valid; add targeted review. If transcript control cannot be maintained, remediate first.

### Weak Mastery

- Repeatedly opens transcript before attempt despite modeling/correction;
- cannot distinguish audio and transcript;
- cannot operate or simulate the protocol with reasonable support;
- or access failure prevents valid audio participation.

**Next action:** Trigger navigation/audio-access or protocol remediation before a dependent listening task. Do not label access failure as weak English.

## 48. Remediation Plan

Do not create remediation lesson now. If future evidence indicates difficulty:

- **Transcript-first habit:** use physical/visual cover, move transcript to a later collapsed section, rehearse sequence with nonlinguistic signal, then parallel token.
- **Audio/transcript confusion:** use two-column sound vs writing model and a one-token comparison.
- **Replay without goal:** present two simple replay targets and require choice before control is enabled.
- **First-attempt record missing:** add a mandatory one-click/checkbox observation before transcript reveal.
- **Label confusion:** restore Vietnamese + icon, contrast only `replay` vs `transcript`, then fade cues.
- **Technical access failure:** verify asset, file path, controls, device volume/output and fallback player; no language score.
- **Fear of being wrong:** emphasize Learn Mode and accept `chưa nhận ra` as valid evidence.

After repair, reassess using a different token/layout, not the memorized original.

## 49. Learner Checklist Plan

Final lesson should include a Vietnamese checklist equivalent to:

- Tôi biết transcript là bản chữ của audio.
- Tôi nghe lần đầu khi transcript vẫn đóng.
- Tôi ghi điều mình nghe được hoặc mức chắc chắn trước khi mở transcript.
- Tôi biết mục tiêu của lần replay.
- Tôi trả lời trước khi xem transcript.
- Tôi dùng transcript để kiểm tra, không dùng nó để thay thế first listen.
- Tôi nghe lại sau khi đối chiếu chữ.
- Tôi phân biệt lỗi kỹ thuật với khó khăn nhận ra English.
- Tôi biết khi nào cần hỗ trợ/remediation.

Checklist is self-report and must be compared with actual protocol evidence.

## 50. One-Minute Summary Plan

Final summary should retain 5–7 essentials:

1. Audio là âm thanh; transcript là bản chữ liên quan.
2. First listen diễn ra trước transcript.
3. Ghi first attempt trước khi support thay đổi response.
4. Replay cần một mục tiêu.
5. Transcript giúp kiểm tra và học sau attempt.
6. Hãy nghe lại sau khi so sánh chữ.
7. Technical failure không phải English listening failure.

Blueprint only identifies content; generator writes polished learner-facing summary.

## 51. Flashcard Plan

**Optional, low priority.**

Useful cards only for functional labels:

- front: `pause`, `replay`, `transcript` or icon;
- back: Vietnamese action/function;
- optional audio for `pause`/`replay` after canonical asset exists.

Do not make flashcards for the full protocol; sequence must be practiced behaviorally.

## 52. Homework Plan

Homework should reinforce without new English:

1. Hide the flow and reconstruct protocol order.
2. Explain in Vietnamese why transcript comes after first attempt.
3. Use one available short audio from Day-002 and perform listen → note → replay goal → response → transcript → re-listen.
4. Classify one hypothetical technical problem separately from one language observation.
5. Record which step still needs help; do not update repository state manually.

If audio asset is unavailable, substitute a process-card simulation and mark listening evidence unavailable. Do not require external audio search.

## 53. Estimated Study Time and Session Splitting

Estimated focused study time: **40–70 minutes**, varying with access setup and repeats. This is not a hard deadline or content limit.

Suggested sessions:

- **Session A — Understand:** safety, R1 retrieval, audio vs transcript, protocol map.
- **Session B — Guided Practice:** worked example and first rehearsal.
- **Session C — Independent Practice:** error correction, second rehearsal, independent mini-protocol.
- **Session D — Mastery and Review:** quiz, self-explanation, checklist, summary and homework.

Each stopping point must occur after a completed activity and must preserve transcript state clearly. All sessions remain one `Day-002` lesson.

## 54. Difficulty Profile

| Dimension | Difficulty | Notes |
|---|---|---|
| Concept | Very Beginner | Process has several steps but only two core distinctions |
| Vocabulary | Very Beginner | `listen` review + `pause`, `replay`, `transcript`; Vietnamese support remains |
| Pronunciation | Awareness only | No IPA, production score or sound rule |
| Listening | Access/ultra-short | Very slow, clear, one-token input |
| Reading | Very Beginner | Short Vietnamese instruction and one-token transcript |
| Production | Very Beginner | Ordering, status choice and Vietnamese self-explanation |
| TOEIC | Not applicable | Awareness note only |
| Overall Cognitive Load | Low linguistic load; moderate procedural load | Scaffold and split sessions if needed |

Only one difficulty dimension should increase at a time. The second rehearsal may reduce prompts, but it must not simultaneously increase speed, length and vocabulary novelty.

## 55. Beginner Safety Check

Before finalizing, verify:

- Vietnamese explains every required action.
- No unexplained English meta-term controls success.
- Transcript is not visible before attempt.
- Audio tokens are known or pre-taught.
- Speech is very slow and clear.
- No grammar or pronunciation knowledge is hidden in an item.
- Status choices are mutually exclusive where exactly one is required.
- `chưa nhận ra` is an acceptable honest response.
- Asset absence has a technical fallback and does not become a score.
- No TOEIC pressure, timing or score claim appears.
- No shame-based language or Vietnamese-learner stereotype appears.

## 56. Duplication Check

### Intentional Retrieval

- Four lesson regions.
- Attempt before feedback.
- `listen` label.
- Technical access vs language performance.

### New Day-002 Content

- Audio vs transcript.
- First-listen record.
- Purposeful replay.
- Transcript reveal and re-listen sequence.
- `pause`, `replay`, `transcript` functional labels.

### Must Avoid

- Recreating Day-001's full four-region/five-step lesson.
- Reusing its exact triangle/star/layout examples.
- Repeating its complete `listen/read/choose/check` quiz.
- Reintroducing Learn Mode as if unseen.
- Copying Day-001's ambiguous audio status pattern.

## 57. Content Accuracy Requirements

Generator must avoid:

- claiming transcript use is always harmful;
- claiming repeated listening alone guarantees improvement;
- equating recognition after transcript with durable independent recognition;
- teaching spelling as exact sound transcription;
- using invalid IPA or pronunciation claims;
- claiming very slow speech is TOEIC speed;
- assuming a missing audio asset exists;
- producing transcript that differs from audio;
- treating a process rehearsal result as CEFR/TOEIC evidence;
- inventing learner errors or mastery.

## 58. Content Boundaries

### Must Cover

- Psychological safety for a zero learner.
- R1 retrieval from Day-001 without full reteaching.
- Audio vs transcript distinction.
- First listen before transcript.
- First-attempt record.
- Purposeful replay.
- Response before reveal.
- Transcript comparison and final re-listen.
- Two rehearsals plus one independent process check.
- Clean technical-access versus language-observation record.
- Detailed feedback, mastery interpretation and remediation route.
- Future review and Day-003 connection.

### May Cover

- Confidence label.
- Optional nonlinguistic access signal.
- Optional repetition of one label after transcript reveal.
- Optional functional-label flashcards.
- Session stopping points.

### Must NOT Expand Into Yet

- Pronunciation lesson, phoneme, IPA, syllable, stress, linking or final sound.
- Alphabet/letter-name teaching.
- Grammar, parts of speech or sentence construction.
- General vocabulary theme.
- Dictation or shadowing as core practice.
- Dialogue, workplace scenario or passage listening.
- Gist/detail/inference strategy at formal depth.
- TOEIC questions, timing or score.
- Error log/review queue lesson content owned by Day-004/005.
- Full feedback-cycle teaching owned by Day-003.
- Any learner-specific result or state update.

## 59. Next-Lesson Connection

**Actual next lesson:** `Day-003 — Cách trả lời và cách dùng feedback`.

Day-002 enables Day-003 because learner can now:

- preserve a response before reveal;
- use delayed transcript as one type of feedback/support;
- compare first attempt with later evidence;
- retry by re-listening after understanding the mismatch.

Knowledge to retain:

- attempt before reveal;
- transcript timing;
- audio vs transcript;
- purposeful replay;
- technical vs language status.

Day-002 must not pre-teach Day-003's full distinction among response, correct answer, explanation and retry formats.

## 60. Generator Instructions

- Viết final lesson chủ yếu bằng clear Vietnamese; preserve only useful English functional labels and gloss them immediately.
- Không giới hạn artificial length; mở rộng explanation/practice đến mức protocol executable, nhưng giới hạn new core load.
- Use exactly the approved scope: protocol and audio–transcript distinction.
- Begin with safety and Day-001 retrieval, not pronunciation theory.
- Keep transcript hidden until after a recorded attempt in every language rehearsal.
- Require a replay goal and final re-listen.
- Use at least two different rehearsal tokens/layouts and one independent mini-protocol.
- Treat audio assets as requirements unless repository confirms they exist.
- If asset/control fails, provide technical branch and do not fabricate Listening evidence.
- Separate mutually exclusive access status from optional language observation.
- Avoid unnecessary English meta-language identified in Day-001 review.
- All checked items need one defensible answer, sufficient context, and why-wrong analysis where alternatives are plausible.
- No grammar, IPA, alphabet, general vocabulary, dictation, dialogue, TOEIC item or score claim.
- End with evidence-report fields for a future State Updater, but do not update files or claim learner mastery.

## 61. Expected Evidence for Future State Updater

Final lesson should make it possible to record, only after actual learner interaction:

- whether audio/control access was verified, unavailable or technically failed;
- whether learner preserved first-attempt evidence;
- whether transcript remained closed until response;
- whether learner used purposeful replay;
- whether learner completed post-transcript re-listen;
- recognition status for `listen`, `pause`, `replay`, `transcript` at intended depth;
- protocol understanding and independence level;
- any actually observed process error;
- recommended R1/R2 review or remediation.

Do not prefill outcomes. Course-content generation is not learner performance.

## 62. Planner Final Audit

- **Next lesson confirmed:** Pass — `COURSE_STATE.md` authorizes Day-002.
- **Remediation override:** Pass — none supported by current state.
- **Prerequisites audited:** Pass — Day-001 content exists; learner performance remains unknown; access check planned.
- **Core scope controlled:** Pass — protocol, transcript distinction, purposeful replay and evidence record only.
- **Vocabulary load:** Pass — one reviewed label, three new functional labels, minimal support terms.
- **Pronunciation progression:** Pass — awareness only, no untaught sound system.
- **Listening progression:** Pass — ultra-short, slow/clear, transcript delayed, final re-listen.
- **Reading progression:** Pass — one-token transcript and Vietnamese instructions only.
- **TOEIC proportionality:** Pass — awareness note only.
- **Retrieval integrated:** Pass — four high-priority Day-001 targets from `REVIEW_QUEUE.md`.
- **No learner data invented:** Pass.
- **Day-001 quality debt handled safely:** Pass — guardrails added without treating defects as learner errors.
- **Duplication prevention:** Pass — retrieval distinguished from new Day-002 scope.
- **Ambiguity prevention:** Pass — access and language statuses separated; unique-answer rules explicit.
- **Next connection:** Pass — prepares Day-003 without generating it.
- **Blueprint executable:** Yes — generator need not decide scope, prerequisites, difficulty, skill integration, review targets or mastery meaning.

