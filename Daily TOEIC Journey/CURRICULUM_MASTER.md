# Daily TOEIC Journey — Chương trình tổng thể

## 1. Mục đích và phạm vi

Tài liệu này chuyển kiến trúc khóa học đã được phê duyệt thành bản đồ chương trình có thể triển khai. Nó xác định các phase, module, hướng phát triển của từng knowledge track, các điểm review/assessment và quy tắc mở rộng chương trình. Tài liệu không chứa bài giảng, bài tập hoàn chỉnh, transcript, đáp án hay dữ liệu thành tích của người học.

Chương trình tuân theo thứ tự thẩm quyền:

1. `COURSE_SPEC.md` — nguyên tắc hiến định và giới hạn toàn khóa;
2. `ROADMAP.md` — mục tiêu phase và điều kiện chuyển phase;
3. `LEARNING_SYSTEM.md` — vòng học, review, mastery và remediation;
4. `KNOWLEDGE_DEPENDENCIES.md` — dependency của kiến thức;
5. `REPOSITORY_ARCHITECTURE.md` — trách nhiệm tệp và quy trình vận hành.

Nếu tài liệu này khác các tệp trên, tệp có thẩm quyền cao hơn được ưu tiên. `Day` là thứ tự bài học, không phải ngày lịch bắt buộc. Không có tổng số ngày cố định cho toàn khóa.

## 2. Nguyên tắc vận hành chương trình

- Tiến trình bắt buộc đi theo: **Learn → Understand → Practice → Apply → Review → Interleave → Assess → Remediate khi cần → Master → Progress**.
- Việc đã học hết nội dung không đồng nghĩa đã mastery.
- Chỉ mở dependency mới khi prerequisite đủ mạnh hoặc có refresher/remediation rõ ràng.
- Giới hạn lượng kiến thức core mới, không giới hạn tùy tiện độ dài giải thích hoặc lượng practice hữu ích.
- General English là nền; TOEIC là môi trường áp dụng ngày càng tăng.
- Review, pronunciation, listening, reading, integration và assessment là thành phần chương trình độc lập, không phải phần trang trí của grammar.
- Vietnamese là ngôn ngữ giải thích chính ở giai đoạn đầu; English exposure tăng dần theo bằng chứng năng lực.
- Không dùng một điểm tổng duy nhất để che lấp weakness ở Listening, Reading, Grammar, Vocabulary hoặc pronunciation perception.
- Remediation là nhánh điều kiện, không mặc định rằng người học luôn pass và cũng không ép mọi lỗi nhỏ thành một bài học mới.

## 3. Active Curriculum Phase

**Active Phase:** Phase 0 — English Orientation  
**CEFR orientation:** Zero → Pre-A1  
**Status:** Expanded  
**Detailed file:** `curriculum/phase-00-english-orientation.md`  
**First lesson:** Day-001

Phase 0 được mở rộng đầy đủ vì đây là phase đang hoạt động. Phase 1–7 chỉ được lập ở cấp module để việc mở rộng sau này có thể phản ánh dữ liệu học thật.

## 4. Bản đồ toàn khóa

### Phase 0 — English Orientation

**CEFR orientation:** Zero → Pre-A1  
**TOEIC orientation:** Chưa đặt mục tiêu điểm; chỉ awareness không chấm điểm  
**Status:** Expanded

**Entry profile:** Có thể chưa biết cách học bằng Markdown/audio, chưa ổn định bảng chữ cái, ranh giới từ, quan hệ chữ–âm, hoặc trật tự subject–verb.

**Exit profile:** Có thể vận hành bài học với hỗ trợ tiếng Việt; nhận ra một ngân hàng từ nền tảng; đọc/nghe câu cực ngắn đã học; xác định ai/cái gì và trạng thái/hành động chính; lắp ráp một số mẫu an toàn.

**Major modules:**

1. P00-M01 — Học cách học và dùng hệ thống.
2. P00-M02 — Chữ viết, âm thanh và ranh giới từ.
3. P00-M03 — Word, phrase, sentence; người và vật.
4. P00-M04 — Subject và câu cơ bản với `be`.
5. P00-M05 — Action verbs và trật tự câu.
6. P00-M06 — Đầu vào chức năng: số, ngày, tên, nhãn và phản hồi ngắn.
7. P00-M07 — Cumulative integration, phase assessment và chuyển tiếp.

**Grammar development:** Sentence core ở mức trực quan; personal pronouns; `am/is/are`; singular/plural idea; `a/an`; câu khẳng định, phủ định và yes/no question với `be` như whole patterns; nhận biết subject + action verb và object.

**Vocabulary development:** Classroom language, greetings, personal information, people, countries, common objects, numbers, days, core actions và basic adjectives. Mục tiêu là form–sound–meaning connection, không phải danh sách từ rời rạc.

**Pronunciation development:** Sound awareness, letter name khác speech sound, syllables, selected high-impact contrasts, final-sound awareness và nghe từ chậm/rõ.

**Listening development:** Dùng audio đúng cách; phân biệt speech/silence; nhận diện letters, digits, names, learned words và câu mẫu rất ngắn.

**Reading development:** Upper/lowercase, spaces, punctuation, word/phrase/sentence, labels, captions, form fields và patterned sentences.

**Paraphrasing development:** Quan hệ cùng nghĩa trực tiếp bằng hình–từ, tên–pronoun và một số greeting/identity patterns; chưa yêu cầu thuật ngữ paraphrase.

**TOEIC development:** Awareness về Listening/Reading và minh họa không chấm điểm; không sử dụng full-format hoặc áp lực thời gian.

**Assessment milestones:** Module checks, cumulative review, phase assessment theo track, và transition check. Weakness lớn dẫn đến remediation theo nhánh.

**Transition requirements:** Đạt exit profile của Phase 0; không còn blocker nghiêm trọng về navigation, print awareness, sound/word recognition hoặc sentence-core awareness; có kế hoạch review cho weakness không chặn tiến trình.

### Phase 1 — Essential Foundations

**CEFR orientation:** Pre-A1 → A1  
**TOEIC orientation:** Familiarization; nếu đo chính thức thường dưới khoảng 250 nhưng độ bất định cao  
**Status:** Planned / Not yet expanded

**Entry profile:** Đã hiểu lesson conventions, một ngân hàng từ nhỏ và một số mẫu câu, nhưng chưa xử lý được câu biến đổi hoặc connected speech tự nhiên.

**Exit profile:** Hiểu và tạo câu khẳng định/phủ định/câu hỏi thông dụng; hiểu basic present meanings; xử lý numbers/schedules; đọc functional texts ngắn và nghe speech rõ về chủ đề quen thuộc.

**Major modules:**

1. P01-M01 — Noun phrase foundation: nouns, plural, countability, `a/an/the`, demonstratives.
2. P01-M02 — Personal reference và `be`: pronouns, possessives, `there is/are`.
3. P01-M03 — Present Simple, core verbs, agreement và daily routines.
4. P01-M04 — Present Continuous, imperatives, `can`, questions và negation.
5. P01-M05 — Place/time language, adjectives, basic adverbs và coordination.
6. P01-M06 — A1 functional texts/listening và gentle TOEIC foundations.
7. P01-M07 — Interleaved review, phase assessment và remediation bridge.

**Grammar development:** Hoàn thiện simple-sentence system; `be` vs lexical verbs; subject/object pronouns; articles; agreement; Present Simple/Continuous; questions, negation, imperatives, `can`, `have/has`, prepositions và `and/but/or`.

**Vocabulary development:** Family, home, routines, food, places, transport, weather, clothing, health basics, time, dates, quantities, jobs và requests; ưu tiên high-frequency vocabulary và recycling đa modality.

**Pronunciation development:** Major vowel/consonant contrasts; final consonants; plural/third-person endings; syllables, word stress và contractions.

**Listening development:** Learned words/phrases → sentences → slow mini-dialogues; numbers, times, prices, dates và basic question types.

**Reading development:** Patterned sentences → signs/forms/messages/schedules → very short paragraphs.

**Paraphrasing development:** Direct synonyms, pronoun reference, simple identity/action restatements và phrase equivalence quen thuộc.

**TOEIC development:** Picture-description foundation, elementary Question–Response intent, simple Part 5 form awareness và very short Part 7-like functional texts.

**Assessment milestones:** Mini checkpoints sau mỗi dependency cluster; listening/reading module checks; phase assessment bằng các module ngắn thay vì một bài kiểm tra kiệt sức.

**Transition requirements:** Simple sentence order, basic verb forms, pronouns/determiners, question comprehension và sound–word recognition đủ ổn định cho A2 work.

### Phase 2 — Elementary Communication

**CEFR orientation:** A1 → A2  
**TOEIC orientation:** Khoảng 200–450  
**Status:** Planned / Not yet expanded

**Entry profile:** Hiểu input ngắn và quen thuộc nhưng còn phụ thuộc vào controlled language, translation và predictable patterns.

**Exit profile:** Kết nối meaning hiện tại/quá khứ/tương lai; xử lý service/travel/workplace exchanges cơ bản; đọc functional texts ngắn; nhận ra simple paraphrases và adapted TOEIC items.

**Major modules:**

1. P02-M01 — Present, past và future time network.
2. P02-M02 — Countability, quantifiers, comparison và description.
3. P02-M03 — Modals, purpose và introductory complementation.
4. P02-M04 — Present Perfect introduction và clause connectors.
5. P02-M05 — Services, travel và introductory workplace communication.
6. P02-M06 — Short connected listening và functional reading genres.
7. P02-M07 — Adapted TOEIC Parts 1, 2, 3/4, 5 và single-text Part 7.
8. P02-M08 — Cumulative assessment và transition.

**Grammar development:** Present Simple/Continuous contrast; Past Simple; future meanings; pronouns/determiners; quantifiers; comparison; modal functions; infinitive purpose; selected gerunds; introductory Present Perfect; basic relative/adverb relations.

**Vocabulary development:** Shopping, travel, hotels, restaurants, appointments, health, technology, office objects, departments, schedules, deliveries và customer requests; bắt đầu collocation grouping.

**Pronunciation development:** Grammatical endings, clusters, word-family stress, sentence focus, common weak forms và linking.

**Listening development:** Sentence cues → short exchanges → simple announcements; question intent, roles, locations và connected auxiliaries.

**Reading development:** Paragraph basics; notices, messages, email, ads, forms, menus và timetables; scanning explicit information.

**Paraphrasing development:** Common synonyms, phrase substitutions, time/quantity restatements và elementary grammatical transformations.

**TOEIC development:** Simplified Parts 1/2, isolated Part 5, adapted Parts 3/4 và short single Part 7; distractor/paraphrase awareness.

**Assessment milestones:** Tense-network checkpoint; functional communication checkpoint; adapted multi-part assessment; phase assessment by subskill.

**Transition requirements:** Basic tense contrasts, noun phrases, questions/modals, connectors, core vocabulary và short-text comprehension đủ chức năng.

### Phase 3 — Functional Intermediate

**CEFR orientation:** A2 → B1  
**TOEIC orientation:** Khoảng 400–600  
**Status:** Planned / Not yet expanded

**Entry profile:** Xử lý predictable communication nhưng giảm mạnh khi câu dài, wording lạ, connected speech hoặc workplace text dày hơn.

**Exit profile:** Hiểu main points và important details của clear workplace conversations/talks và multi-paragraph texts; kiểm soát core B1 grammar; làm mọi TOEIC part ở mức adapted/moderate.

**Major modules:**

1. P03-M01 — Tense/aspect consolidation và discourse time.
2. P03-M02 — Modal meanings, passive voice và complementation.
3. P03-M03 — Relative, noun và adverb clauses; conditionals/reporting introduction.
4. P03-M04 — Workplace core domains và business correspondence.
5. P03-M05 — Multi-turn conversations, talks, intent và basic inference.
6. P03-M06 — Medium texts, reference, purpose và evidence-based inference.
7. P03-M07 — All TOEIC Parts 1–7 at adapted/moderate level.
8. P03-M08 — Untimed competence assessment plus short timed sampling.

**Grammar development:** Tense/aspect system; Present Perfect vs Past Simple; passive; infinitive/gerund patterns; participles; relative/noun/adverb clauses; first/second conditionals; introductory reported speech.

**Vocabulary development:** Meetings, recruitment, HR, training, purchasing, sales, marketing, banking, events, logistics, shipping, policies, facilities và maintenance; word families/collocations tăng dần.

**Pronunciation development:** Thought groups, prominence, reductions, linking/assimilation patterns, stress shifts và controlled accent exposure.

**Listening development:** Multi-turn conversation/talk; sequence, purpose, intention, problem/solution, paraphrase, distractor rejection và selective notes.

**Reading development:** Medium emails/articles; sentence parsing under modification; reference, main idea, purpose, details và basic document relationships.

**Paraphrasing development:** Lexical networks, word-family shifts, active/passive equivalence và clause/phrase reformulation.

**TOEIC development:** Mọi part được dạy theo micro-skill; Part 5 form/meaning, Part 6 local cohesion, Part 7 singles và short Parts 3/4 sets; light timing.

**Assessment milestones:** Grammar/discourse checkpoint; workplace vocabulary transfer; listening/reading cumulative checks; phase assessment tách competence và timed performance.

**Transition requirements:** Clause boundaries, tense/aspect, passive/complex-clause foundations, functional B1 vocabulary và independent single-passage comprehension.

### Phase 4 — Independent Bridge

**CEFR orientation:** B1 → B1+/early B2  
**TOEIC orientation:** Khoảng 550–730  
**Status:** Planned / Not yet expanded

**Entry profile:** Hiểu connected English rõ nhưng chậm trước dense noun phrases, indirect meaning, unfamiliar paraphrase, multiple speakers và linked documents.

**Exit profile:** Xử lý workplace discourse khá độc lập, kết nối hai nguồn, suy ra intention hợp lý, nhận nhiều dạng paraphrase và duy trì short timed TOEIC sets.

**Major modules:**

1. P04-M01 — Perfect/progressive precision và advanced passive.
2. P04-M02 — Relative reduction, participial/infinitive modification và causatives.
3. P04-M03 — Condition, reporting, complex comparison và adverbial relations.
4. P04-M04 — Business-domain expansion, polysemy, register và paraphrase networks.
5. P04-M05 — Three-speaker listening, attitude, implied action và graphics.
6. P04-M06 — Dense texts, double passages, cohesion, tone và inference.
7. P04-M07 — Mixed TOEIC sets và timed clusters.
8. P04-M08 — Cumulative bridge assessment và remediation.

**Grammar development:** Greater tense/aspect precision; passive with modals; relative omission/reduction; non-finite modification; causatives; third/mixed conditional concepts; reported speech; advanced determiners/comparison.

**Vocabulary development:** Contracts, finance, insurance, manufacturing, construction, real estate, healthcare, projects, performance và policy; collocation, register và polysemy.

**Pronunciation development:** Authentic-rate transformations, contrastive stress, reduced syllables, boundary detection và perception-led shadowing.

**Listening development:** Less predictable exchanges; three speakers; attitude, implied action, graphics, faster details và accent variation.

**Reading development:** Dense email/article; sentence insertion; writer purpose/tone; double documents và faster evidence location.

**Paraphrasing development:** Grammar transformations, business equivalents, time/cause/condition reformulation và indirect statements.

**TOEIC development:** Regular mixed sets, moderate Parts 3–7, timed clusters, distractor taxonomy và evidence-based pacing experiments.

**Assessment milestones:** Double-source checkpoint; connected-speech/perception check; short timed sections; bridge phase assessment.

**Transition requirements:** Independent B1 comprehension, emerging B2 discourse control, durable workplace lexis và stable strategy under moderate timing.

### Phase 5 — Upper-Intermediate Performance

**CEFR orientation:** B1+ → B2  
**TOEIC orientation:** Khoảng 700–850  
**Status:** Planned / Not yet expanded

**Entry profile:** Độc lập trong familiar domains nhưng cần tăng speed, precision, inference và control của dense/indirect language.

**Exit profile:** Hiểu complex main ideas và phần lớn important details; reasoning across documents; xử lý indirect/paraphrased meaning; có performance ổn định ở realistic TOEIC sections.

**Major modules:**

1. P05-M01 — Complex verb phrases, modality và stance.
2. P05-M02 — Advanced passives, causatives, complementation và reduced clauses.
3. P05-M03 — Advanced modification, ellipsis/substitution và discourse connectors.
4. P05-M04 — B2 cross-domain lexicon, derivation, collocation và near-synonyms.
5. P05-M05 — Authentic-rate listening, graphics, attitude và paraphrase chains.
6. P05-M06 — Long texts, triple-passage introduction và cross-document updates.
7. P05-M07 — Realistic Parts 1–7, timed part clusters và section training.
8. P05-M08 — First readiness-based full simulations và B2 assessment.

**Grammar development:** Complex verb phrases; stance; advanced voice/causatives; non-finite/reduced structures; modification; receptive inversion; ellipsis và cohesion.

**Vocabulary development:** Broad business plus abstract general vocabulary; derivational morphology, near-synonyms, collocational constraints và test-appropriate idiomatic language.

**Pronunciation development:** Rapid connected-speech parsing, accent accommodation, focus/implication và individualized perception repair.

**Listening development:** Authentic-rate conversations/talks; indirect requests, attitude, multi-step intent, graphics và longer retention.

**Reading development:** Longer articles/document sets; dense noun phrases; triple passages; contradiction/update tracking; strategic skimming/scanning.

**Paraphrasing development:** Multi-step lexical/grammar chains, active/passive and nominalized equivalents, nuanced near-equivalence and implication boundaries.

**TOEIC development:** Realistic item structure; timed clusters/sections; speed–accuracy analysis; first full simulations only when ready.

**Assessment milestones:** B2 language assessment; listening/reading transfer checks; timed sections; multiple evidence points before durable status.

**Transition requirements:** Broad B2 readiness, automatic core grammar/lexis, near-test-speed listening, multi-document reading và section stamina.

### Phase 6 — Advanced TOEIC Integration

**CEFR orientation:** B2 → B2+  
**TOEIC orientation:** Khoảng 800–930  
**Status:** Planned / Not yet expanded

**Entry profile:** General comprehension và test familiarity mạnh; điểm mất tập trung ở nuance, dense paraphrase, subtle inference, speed hoặc personal error patterns.

**Exit profile:** Performance mạnh và nhất quán ở mọi TOEIC part; xác định root causes; giữ accuracy trong realistic timing; hiểu varied B2+ input.

**Major modules:**

1. P06-M01 — Precision grammar, ambiguity và compressed structures.
2. P06-M02 — Advanced business/general lexicon, nominalization và register.
3. P06-M03 — High-speed listening, speaker switching và subtle intent.
4. P06-M04 — Dense triple documents, chronology và implicit purpose.
5. P06-M05 — Individual bottleneck laboratories by error profile.
6. P06-M06 — Timed sections, pacing, stamina và confidence calibration.
7. P06-M07 — Full simulations, parallel forms và transfer verification.

**Grammar development:** Long-distance reference/agreement; compressed modification; advanced reduction; condition/concession/stance; formal patterns và selected high-value exceptions.

**Vocabulary development:** Nuanced collocations, polysemy, register, nominalization, idiomatic paraphrase và domain transfer; low-frequency selection theo coverage value.

**Pronunciation development:** Individualized weak-form, boundary, accent và prosodic diagnostics.

**Listening development:** High-speed natural delivery; distractor timing, implied action, dense details và robust accent adaptation.

**Reading development:** Rapid structural parsing; long dense documents; triple-source chronology/reference và efficient evidence verification.

**Paraphrasing development:** Nuanced contextual equivalence, stance, implication, nominalized reformulation và multi-sentence paraphrase.

**TOEIC development:** Timed sections/full simulations; part-specific pacing; data-driven item targeting; endurance/recovery routines.

**Assessment milestones:** Rolling timed evidence, parallel-form transfer, error-pattern review và standardized mock conditions.

**Transition requirements:** Durable B2, independent remediation, stable high-level TOEIC evidence trên hơn một form và readiness cho C1-oriented breadth.

### Phase 7 — C1-Oriented Mastery

**CEFR orientation:** B2+ → approximately C1  
**TOEIC orientation:** Thường có tiềm năng 850–990 nhưng rất cá nhân và có ceiling effects  
**Status:** Planned / Not yet expanded

**Entry profile:** Đã xử lý phần lớn TOEIC và broad B2 input; cần phát triển nuance, breadth, complex discourse và autonomous exposure.

**Exit profile:** Hiểu demanding extended input, implicit relationships, multiple-source synthesis, nuance/stance và high-speed performance; tự quản lý việc học tiếp theo.

**Major modules:**

1. P07-M01 — Advanced discourse grammar, information structure và rhetorical relations.
2. P07-M02 — C1-oriented general/professional/selected academic lexicon.
3. P07-M03 — Information-dense, multi-speaker và unfamiliar-topic listening.
4. P07-M04 — Complex long-form reading, evaluation và sophisticated synthesis.
5. P07-M05 — TOEIC ceiling precision và maintenance.
6. P07-M06 — Independent projects, transfer và lifelong learning systems.
7. P07-M07 — Combined C1-oriented profile assessment and course completion review.

**Grammar development:** Nuanced modality/stance, information structure, cohesion/coherence, ellipsis, substitution, inversion, emphasis, compressed formal style và ambiguity resolution.

**Vocabulary development:** Connotation, register, semantic prosody, metaphorical extension, professional và selected academic vocabulary, sophisticated paraphrase networks.

**Pronunciation development:** Broader accent/style perception, prosodic nuance và intelligibility maintenance khi productive work được dùng.

**Listening development:** Fast, dense, multi-speaker input; implicit agreement/disagreement; relevant idiomatic nuance; synthesis và selective notes.

**Reading development:** Complex long-form professional/general texts; rhetoric, subtle tone, synthesis, evaluation và partially implicit cross-document links.

**Paraphrasing development:** Discourse-level reformulation, connotation/stance preservation, implication constraints và synthesis across sources.

**TOEIC development:** Ceiling precision, efficient verification, rare-error analysis và periodic maintenance; không lặp vô hạn easy drills.

**Assessment milestones:** Broad C1-oriented receptive tasks plus TOEIC simulations; long-interval transfer; autonomous self-explanation.

**Transition requirements:** Đây là phase cuối. Completion cần broad evidence chứ không chỉ high TOEIC score; sau đó chuyển sang maintenance và authentic use.

## 5. Các progression spine toàn khóa

### 5.1 Grammar

```text
print/sound conventions
→ word / phrase / sentence
→ subject + verb + basic word order
→ nouns / pronouns / be / simple patterns
→ lexical verbs / agreement / questions / negation
→ tense and aspect network
→ modals / complements / passive
→ relative, noun and adverb clauses
→ conditionals / reported speech
→ reduced clauses / advanced modification
→ discourse grammar / stance / information structure
```

Mỗi concept được dạy theo form–meaning–use, sau đó contrast và interleave. Technical terms chỉ trở thành prerequisite sau khi đã được giải thích bằng ngôn ngữ phù hợp.

### 5.2 Vocabulary

```text
learning how to learn a word
→ high-frequency foundation
→ everyday semantic networks
→ functional services/travel
→ workplace core
→ TOEIC domain expansion
→ cross-domain precision and paraphrase
→ advanced general/professional/selected academic vocabulary
```

Vocabulary được theo dõi theo written recognition, spoken recognition, contextual meaning, delayed retrieval, collocation/paraphrase và production khi có mục tiêu. Review phải đổi context/modality thay vì lặp nguyên danh sách.

### 5.3 Pronunciation

```text
sound awareness
→ selected vowels/consonants
→ final consonants and clusters
→ grammatical endings
→ syllables and word stress
→ sentence stress and thought groups
→ linking, weak forms and reductions
→ accent/rate variation and prosodic meaning
```

Mục tiêu chính là listening perception và intelligibility, không phải bắt chước một accent duy nhất.

### 5.4 Listening

```text
sound
→ word
→ phrase boundary
→ sentence
→ question–response
→ short exchange
→ conversation / talk
→ multiple speakers / graphics
→ paraphrase / inference
→ high-speed dense natural speech
```

Transcript chỉ được reveal sau attempt theo declared mode. Dictation/shadowing dùng khi đúng root cause.

### 5.5 Reading

```text
word
→ phrase
→ sentence core
→ short paragraph
→ notice / message / form / schedule
→ email / advertisement / article
→ business document
→ double passage
→ triple passage
→ advanced synthesis and inference
```

Speed work chỉ tăng sau khi parsing tương đối chính xác. Inference luôn phải có evidence bridge.

### 5.6 Paraphrasing

```text
same picture/name reference
→ simple synonyms
→ common phrase equivalence
→ word-family transformation
→ grammatical transformation
→ active/passive and cause/effect
→ indirect meaning and stance
→ discourse-level and inference-based equivalence
```

Mỗi equivalence phải bảo toàn participant, action/state, time, polarity, degree, condition và certainty ở mức liên quan.

### 5.7 TOEIC

```text
general English foundation
→ TOEIC awareness
→ TOEIC-style examples
→ isolated questions
→ small homogeneous sets
→ mixed untimed sets
→ short timed clusters
→ timed parts/sections
→ full simulations and score-range evidence
```

TOEIC score bands chỉ là planning orientation, không phải CEFR conversion hay guarantee.

## 6. Review và interleaving toàn khóa

- **R0:** retrieval trong cùng lesson/session.
- **R1:** lesson kế tiếp.
- **R2:** khoảng 3–4 lesson/study days sau, điều chỉnh theo evidence.
- **R3:** khoảng một tuần học sau.
- **R4:** khoảng hai tuần.
- **R5:** khoảng một tháng.
- **Maintenance:** khoảng cách dài dần cho knowledge durable.

Active phase curriculum phải chỉ rõ các review targets ở cấp lesson. Future phase expansion phải bố trí module review, cumulative review và phase review. Interleaving tiến từ blocked practice → near contrast → local mix → cross-track mix → unlabeled context → timed transfer. Không mix quá sớm trước khi người học hiểu concept riêng lẻ.

## 7. Assessment và remediation strategy

### Assessment layers

- Prerequisite diagnostic trước dependency quan trọng.
- Lesson mastery evidence sau mỗi bài.
- Module mini-checkpoint khi đã có một cluster đáng đánh giá.
- Periodic cumulative review kiểm tra delayed retrieval và transfer.
- Phase assessment kiểm tra exit capabilities theo từng track.
- TOEIC-mode assessment chỉ tăng khi format, language và timing readiness phù hợp.

### Interpretation

- **Strong:** accuracy tốt, giải thích/nhận diện meaning đúng, độc lập, transfer được và giữ sau delay.
- **Partial:** core meaning đúng nhưng còn chậm, thiếu ổn định hoặc yếu trong một số context.
- **Weak:** misunderstanding lặp lại, phụ thuộc hỗ trợ nặng hoặc prerequisite không dùng được.

Không dùng một threshold số duy nhất cho mọi skill. Assessment quan trọng cần nhiều evidence points.

### Remediation branches

- Foundation/navigation weakness → hướng dẫn lại thao tác với lesson/audio/transcript.
- Print/sound weakness → smaller discrimination steps và sound–text mapping.
- Vocabulary retention weakness → contextual retrieval và interval adjustment.
- Grammar misconception → alternative explanation, contrast và guided rebuild.
- Listening weakness → xác định sound, boundary, vocabulary hay memory cause.
- Reading weakness → quay lại word/phrase/sentence parsing trước speed.
- TOEIC timing weakness → so sánh timed/untimed evidence trước khi chỉnh pacing.

Remediation không chiếm Day ID cố định nếu chưa xảy ra. Sau remediation phải reassess bằng parallel task, không chỉ lặp nguyên item.

## 8. Quy tắc mở rộng phase tương lai

Khi người học gần hoàn thành active phase:

1. đọc đầy đủ năm architecture files;
2. đọc `CURRICULUM_MASTER.md` và detailed curriculum của active phase;
3. đọc `COURSE_STATE.md`, state chi tiết, review queue, error patterns và actual assessment evidence;
4. xác nhận exit capabilities và unresolved prerequisites;
5. mở rộng **chỉ phase kế tiếp** thành modules và sequential Day lessons;
6. bắt đầu Day ID sau Day cuối đã release, không tái sử dụng ID;
7. điều chỉnh allocation theo strengths/weaknesses thật nhưng giữ phase objectives và dependency order;
8. independent-review curriculum sequence trước khi active;
9. cập nhật status: phase hoàn thành giữ lịch sử; next phase chuyển thành `Expanded`/active.

Không sửa lịch sử lesson đã hoàn thành trừ curriculum correction có version, rationale, impact analysis và migration note. Nếu lesson scope thay đổi materially, phải dùng versioning/ID policy theo repository architecture.

## 9. Quy tắc adaptation

- Strong grammar + weak listening: giữ grammar progression nhưng tăng perception, repeated-context listening và sound–text repair; không bỏ grammar prerequisites.
- Weak vocabulary retention: giảm lượng lexical core mới, tăng retrieval diversity và cross-context recycling.
- Strong reading + weak pronunciation: không dùng reading score để cho qua listening prerequisite.
- Strong untimed + weak timed performance: giữ Learn Mode cho bottleneck, thêm fluency/timing từng bước.
- Weak prerequisite hub: tạm dừng dependent lessons trực tiếp; các track không phụ thuộc vẫn có thể tiếp tục.
- Minor isolated errors: tiếp tục với targeted review, tránh endless remediation.

Mọi adaptation phải được ghi trong state/plan, không tồn tại chỉ trong conversation memory. Không adaptation nào được phá vỡ course vision, phase exit profile hoặc prerequisite graph.

## 10. Trách nhiệm của Lesson Planner

Lesson Planner phải đọc architecture, active curriculum lesson entry và current state; xác nhận prerequisite; biến planning metadata thành lesson plan; giữ đúng Core/Useful/Future roles; thiết kế practice progression; chỉ yêu cầu audio chứ không tự giả định audio có sẵn; và nêu evidence cần chuyển cho State Updater.

Lesson Planner không được:

- mở rộng primary scope ngoài curriculum mà không có curriculum correction;
- biến future preview thành mastery target;
- coi exposure là mastery;
- tạo full-speed TOEIC work ở phase không sẵn sàng;
- bỏ review targets hoặc remediation decision;
- dùng knowledge chưa dạy mà không explain/reference/label optional.
