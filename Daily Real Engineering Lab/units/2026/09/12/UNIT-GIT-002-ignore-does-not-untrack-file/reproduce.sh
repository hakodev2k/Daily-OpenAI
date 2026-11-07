#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "$0")" && pwd)"
"$ROOT/starter/setup-repo.sh" >/dev/null
cd "$ROOT/workspace/repo"

tracked=0
if git ls-files --error-unmatch appsettings.Development.json >/dev/null 2>&1; then
  tracked=1
fi

status="$(git status --short -- appsettings.Development.json)"
ignored=0
if git check-ignore -q appsettings.Development.json; then
  ignored=1
fi

printf 'tracked=%s\nignored=%s\nstatus=%s\n' "$tracked" "$ignored" "$status"

if [[ "$tracked" -eq 1 && -n "$status" ]]; then
  echo "PASS: symptom reproduced — local settings are still reported as a tracked modification."
  exit 0
fi

echo "FAIL: expected tracked-file symptom was not reproduced."
exit 1
