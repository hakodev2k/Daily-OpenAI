#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "$0")" && pwd)"
REPO="$ROOT/workspace/repo"

if [[ ! -d "$REPO/.git" ]]; then
  echo "FAIL: run ./reproduce.sh first."
  exit 1
fi

cd "$REPO"

if [[ ! -f appsettings.Development.json ]]; then
  echo "FAIL: local settings file must remain on disk."
  exit 1
fi

if ! git check-ignore -q appsettings.Development.json; then
  echo "FAIL: file is not covered by ignore rules."
  exit 1
fi

if git ls-files --error-unmatch appsettings.Development.json >/dev/null 2>&1; then
  echo "FAIL: file is still tracked in the index."
  exit 1
fi

if ! grep -q 'local.partner.test' appsettings.Development.json; then
  echo "FAIL: local working copy was not preserved."
  exit 1
fi

echo "PASS: local file is preserved, ignored, and no longer tracked."
