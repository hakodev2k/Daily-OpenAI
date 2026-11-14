import pathlib, sqlite3, sys
base = pathlib.Path(__file__).resolve().parent
conn = sqlite3.connect(":memory:")
conn.executescript((base / "schema.sql").read_text(encoding="utf-8"))
query = (base / "query.sql").read_text(encoding="utf-8")
try:
    rows = conn.execute(query).fetchall()
except sqlite3.Error as exc:
    print(f"SQL_ERROR: {exc}", file=sys.stderr)
    sys.exit(2)
ids = [r[0] for r in rows]
print("RESULT_IDS:", ids)
mode = sys.argv[1] if len(sys.argv) > 1 else "run"
expected = [101, 103]
if mode == "reproduce":
    if ids != []:
        print("REPRODUCE_FAILED: original symptom not present")
        sys.exit(1)
    print("REPRODUCED: eligible households missing")
elif mode == "verify":
    if ids != expected:
        print("VERIFY_FAILED: expected", expected, "got", ids)
        sys.exit(1)
    # Independent negative checks on the actual learner query result.
    if len(ids) != len(set(ids)) or any(i in (102, 105, 104) for i in ids):
        print("VERIFY_FAILED: duplicates or excluded household")
        sys.exit(1)
    print("VERIFIED: expected eligible recipients and exclusions")
