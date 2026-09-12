#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
REPO="$ROOT/workspace/repo"

rm -rf "$REPO"
mkdir -p "$REPO"
cd "$REPO"

git init -q
git config user.email "lab@example.test"
git config user.name "Engineering Lab"

cat > appsettings.Development.json <<'JSON'
{
  "PartnerEndpoint": "https://sandbox.partner.test"
}
JSON

git add appsettings.Development.json
git commit -q -m "Add development settings"

printf 'appsettings.Development.json\n' > .gitignore
git add .gitignore
git commit -q -m "Ignore local development settings"

cat > appsettings.Development.json <<'JSON'
{
  "PartnerEndpoint": "https://local.partner.test"
}
JSON

printf 'Workspace prepared at %s\n' "$REPO"
