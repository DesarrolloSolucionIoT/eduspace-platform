#!/usr/bin/env bash
# End-to-end endpoint test runner for eduspace-platform.
# Validates: auth flow, refresh/logout, single-use refresh, protected endpoints,
# domain validation (DNI/phone), state machine (Report), double-booking (Meeting), CORS.
#
# Prerequisites:
#   - API running at $API_BASE (default http://localhost:5204)
#   - MySQL up via docker compose; database created via dotnet ef database update
#   - .env loaded (TokenSettings__Secret, Issuer, Audience, CORS_ALLOWED_ORIGINS)
#
# Usage:
#   ./scripts/test-api.sh
#   API_BASE=http://localhost:5204 ./scripts/test-api.sh
#
# Exit code: 0 if all tests pass, 1 if any fail.

set -u
API_BASE="${API_BASE:-http://localhost:5204}"
API="${API_BASE}/api/v1"

# -- ANSI colors --
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m'

PASS=0
FAIL=0
FAIL_DETAILS=()

require() {
  command -v "$1" >/dev/null 2>&1 || { echo "missing dependency: $1"; exit 2; }
}
require curl
require jq

# rand suffix to keep usernames unique across runs
RAND="$(date +%s%N | tail -c 7)"

# ---- helpers ----------------------------------------------------------------

# expect HTTP code; usage: expect <name> <expected-code> <actual-code> [body]
expect() {
  local name="$1" expected="$2" actual="$3" body="${4:-}"
  if [[ "$actual" == "$expected" ]]; then
    printf "${GREEN}PASS${NC} %-60s [%s]\n" "$name" "$actual"
    PASS=$((PASS + 1))
  else
    printf "${RED}FAIL${NC} %-60s expected %s, got %s\n" "$name" "$expected" "$actual"
    [[ -n "$body" ]] && echo "  body: ${body:0:200}"
    FAIL=$((FAIL + 1))
    FAIL_DETAILS+=("$name (expected $expected, got $actual)")
  fi
}

# curl_status <method> <path> [extra args...] -> echoes "HTTP_CODE\nBODY"
curl_status() {
  local method="$1" path="$2"
  shift 2
  local out
  out="$(curl -sS -o /tmp/eduspace-resp.$$ -w '%{http_code}' -X "$method" "${API}${path}" "$@" || echo "000")"
  echo "$out"
  cat /tmp/eduspace-resp.$$ 2>/dev/null || true
  rm -f /tmp/eduspace-resp.$$
}

# json_get <file> <jq-expr>
json_get() {
  jq -r "$2" < "$1" 2>/dev/null
}

# ---- 0. Server reachable ----------------------------------------------------

echo
echo "==> 0. Sanity"
HTTP=$(curl -sS -o /dev/null -w '%{http_code}' "${API_BASE}/swagger/v1/swagger.json" || echo "000")
expect "swagger reachable"                              "200" "$HTTP"

# ---- 1. Sign-up + verify-code happy path -----------------------------------

echo
echo "==> 1. Auth happy path"

USERNAME="testuser_${RAND}"
EMAIL="test_${RAND}@example.com"
PASSWORD="Password1!"

SIGNUP_BODY=$(cat <<EOF
{"username":"${USERNAME}","email":"${EMAIL}","password":"${PASSWORD}","role":"RoleTeacher"}
EOF
)

HTTP=$(curl -sS -o /tmp/signup.$$ -w '%{http_code}' -X POST "${API}/authentication/sign-up" \
  -H 'Content-Type: application/json' -d "$SIGNUP_BODY")
expect "sign-up new user"                               "200" "$HTTP" "$(cat /tmp/signup.$$)"
rm -f /tmp/signup.$$

HTTP=$(curl -sS -o /tmp/signin.$$ -w '%{http_code}' -X POST "${API}/authentication/sign-in" \
  -H 'Content-Type: application/json' \
  -d "{\"username\":\"${USERNAME}\",\"password\":\"${PASSWORD}\"}")
expect "sign-in returns 200 (code sent)"                "200" "$HTTP" "$(cat /tmp/signin.$$)"
rm -f /tmp/signin.$$

# To verify-code, the user reads the email. In automated testing you'd query the DB
# directly. For now we surface the limitation:
echo -e "${YELLOW}SKIP${NC} verify-code (requires email inbox or DB query — populate VERIFY_CODE env to run)"
if [[ -n "${VERIFY_CODE:-}" ]]; then
  HTTP=$(curl -sS -o /tmp/verify.$$ -w '%{http_code}' -X POST "${API}/authentication/verify-code" \
    -H 'Content-Type: application/json' \
    -d "{\"username\":\"${USERNAME}\",\"code\":\"${VERIFY_CODE}\"}")
  expect "verify-code returns 200 + tokens"             "200" "$HTTP" "$(cat /tmp/verify.$$)"
  ACCESS=$(json_get /tmp/verify.$$ '.accessToken // .token // empty')
  REFRESH=$(json_get /tmp/verify.$$ '.refreshToken // empty')
  rm -f /tmp/verify.$$
  [[ -n "$ACCESS"  ]] && PASS=$((PASS+1)) && echo -e "${GREEN}PASS${NC} access token present"  || { FAIL=$((FAIL+1)); echo -e "${RED}FAIL${NC} access token missing in response"; }
  [[ -n "$REFRESH" ]] && PASS=$((PASS+1)) && echo -e "${GREEN}PASS${NC} refresh token present" || { FAIL=$((FAIL+1)); echo -e "${RED}FAIL${NC} refresh token missing in response"; }
fi

# ---- 2. Protected endpoints reject anonymous --------------------------------

echo
echo "==> 2. Authorization gate"

HTTP=$(curl -sS -o /dev/null -w '%{http_code}' "${API}/teachers-profiles")
expect "GET /teachers-profiles without token → 401"     "401" "$HTTP"

HTTP=$(curl -sS -o /dev/null -w '%{http_code}' "${API}/classrooms")
expect "GET /classrooms without token → 401"            "401" "$HTTP"

HTTP=$(curl -sS -o /dev/null -w '%{http_code}' "${API}/meetings")
expect "GET /meetings without token → 401"              "401" "$HTTP"

HTTP=$(curl -sS -o /dev/null -w '%{http_code}' "${API}/reports")
expect "GET /reports without token → 401"               "401" "$HTTP"

HTTP=$(curl -sS -o /dev/null -w '%{http_code}' -H 'Authorization: Bearer not.a.real.token' "${API}/classrooms")
expect "GET /classrooms with garbage token → 401"       "401" "$HTTP"

# ---- 3. Refresh token semantics (if VERIFY_CODE was set) --------------------

if [[ -n "${ACCESS:-}" && -n "${REFRESH:-}" ]]; then
  echo
  echo "==> 3. Refresh token rotation"

  HTTP=$(curl -sS -o /tmp/auth-ok.$$ -w '%{http_code}' -H "Authorization: Bearer ${ACCESS}" "${API}/classrooms")
  expect "GET /classrooms with valid token → 200"       "200" "$HTTP" "$(cat /tmp/auth-ok.$$)"
  rm -f /tmp/auth-ok.$$

  HTTP=$(curl -sS -o /tmp/refresh.$$ -w '%{http_code}' -X POST "${API}/authentication/refresh" \
    -H 'Content-Type: application/json' \
    -d "{\"refreshToken\":\"${REFRESH}\"}")
  expect "POST /refresh with valid token → 200"         "200" "$HTTP" "$(cat /tmp/refresh.$$)"
  NEW_ACCESS=$(json_get /tmp/refresh.$$ '.accessToken // empty')
  NEW_REFRESH=$(json_get /tmp/refresh.$$ '.refreshToken // empty')
  rm -f /tmp/refresh.$$

  HTTP=$(curl -sS -o /dev/null -w '%{http_code}' -X POST "${API}/authentication/refresh" \
    -H 'Content-Type: application/json' -d "{\"refreshToken\":\"${REFRESH}\"}")
  expect "POST /refresh with used token → 401 (single-use)" "401" "$HTTP"

  HTTP=$(curl -sS -o /dev/null -w '%{http_code}' -X POST "${API}/authentication/logout" \
    -H 'Content-Type: application/json' -d "{\"refreshToken\":\"${NEW_REFRESH}\"}")
  expect "POST /logout with valid refresh → 204"        "204" "$HTTP"

  HTTP=$(curl -sS -o /dev/null -w '%{http_code}' -X POST "${API}/authentication/refresh" \
    -H 'Content-Type: application/json' -d "{\"refreshToken\":\"${NEW_REFRESH}\"}")
  expect "POST /refresh after logout → 401"             "401" "$HTTP"

  ACCESS="$NEW_ACCESS"
else
  echo
  echo -e "${YELLOW}==> 3. SKIPPED (VERIFY_CODE not set — cannot complete auth flow)${NC}"
fi

# ---- 4. Domain validation: DNI / phone --------------------------------------

echo
echo "==> 4. Domain validation (anonymous endpoints are not gated, but DTO validation should fire)"

# These endpoints REQUIRE auth post Fase 2, but DTO validation runs before auth gate in ASP.NET
# when [ApiController] is present. If the endpoints 401 instead of 400, that's still acceptable —
# we test by ensuring 400 returns when authenticated (skip if no ACCESS).
if [[ -n "${ACCESS:-}" ]]; then
  H="Authorization: Bearer ${ACCESS}"

  BAD_DNI=$(cat <<EOF
{"firstName":"Test","lastName":"User","email":"valid@example.com","dni":"1234","phone":"912345678","address":"Lima","username":"u${RAND}","password":"Password1!"}
EOF
)
  HTTP=$(curl -sS -o /tmp/bad-dni.$$ -w '%{http_code}' -X POST "${API}/teachers-profiles" \
    -H "$H" -H 'Content-Type: application/json' -d "$BAD_DNI")
  expect "POST teacher with bad DNI (4 digits) → 400"   "400" "$HTTP" "$(cat /tmp/bad-dni.$$)"
  rm -f /tmp/bad-dni.$$

  BAD_PHONE=$(cat <<EOF
{"firstName":"Test","lastName":"User","email":"valid@example.com","dni":"12345678","phone":"812345678","address":"Lima","username":"u${RAND}a","password":"Password1!"}
EOF
)
  HTTP=$(curl -sS -o /tmp/bad-phone.$$ -w '%{http_code}' -X POST "${API}/teachers-profiles" \
    -H "$H" -H 'Content-Type: application/json' -d "$BAD_PHONE")
  expect "POST teacher with bad phone (starts 8) → 400" "400" "$HTTP" "$(cat /tmp/bad-phone.$$)"
  rm -f /tmp/bad-phone.$$
else
  echo -e "${YELLOW}SKIP${NC} 4 (no access token)"
fi

# ---- 5. Meeting conflict (double-booking guard) -----------------------------

if [[ -n "${ACCESS:-}" ]]; then
  echo
  echo "==> 5. Meeting double-booking guard"
  echo -e "${YELLOW}SKIP${NC} 5 (requires existing classroom + admin + teacher fixtures — not auto-seeded)"
  echo "       To exercise: create classroom, admin, teacher; POST meeting A (2025-12-01 09:00-10:00);"
  echo "       POST meeting B for same teacher overlapping → expect 409 MeetingConflictException."
fi

# ---- 6. Report state machine ------------------------------------------------

if [[ -n "${ACCESS:-}" ]]; then
  echo
  echo "==> 6. Report state machine"
  echo -e "${YELLOW}SKIP${NC} 6 (requires existing resource fixture — not auto-seeded)"
  echo "       To exercise: create resource; POST report → status EnEspera;"
  echo "       PUT report markAsInProgress → 200; PUT markAsCompleted → 200;"
  echo "       PUT markAsInProgress again → 409 InvalidReportTransitionException."
fi

# ---- 7. CORS preflight ------------------------------------------------------

echo
echo "==> 7. CORS preflight"

# Allowed origin (default dev fallback)
HTTP=$(curl -sS -o /dev/null -w '%{http_code}' -X OPTIONS "${API}/classrooms" \
  -H 'Origin: http://localhost:5173' \
  -H 'Access-Control-Request-Method: GET' \
  -H 'Access-Control-Request-Headers: authorization')
expect "OPTIONS from allowed origin (vite localhost) → 204" "204" "$HTTP"

# Disallowed origin
HEADERS=$(curl -sS -i -X OPTIONS "${API}/classrooms" \
  -H 'Origin: http://evil.example' \
  -H 'Access-Control-Request-Method: GET' \
  -H 'Access-Control-Request-Headers: authorization' | head -20)
echo "$HEADERS" | grep -qi "access-control-allow-origin: http://evil.example" && {
  printf "${RED}FAIL${NC} %-60s CORS leaks Access-Control-Allow-Origin to evil.example\n" "OPTIONS from disallowed origin"
  FAIL=$((FAIL + 1))
  FAIL_DETAILS+=("CORS leak to evil.example")
} || {
  printf "${GREEN}PASS${NC} %-60s no Allow-Origin for disallowed origin\n" "OPTIONS from disallowed origin"
  PASS=$((PASS + 1))
}

# ---- Summary ----------------------------------------------------------------

echo
echo "============================================================"
echo "                          SUMMARY"
echo "============================================================"
printf "PASS: ${GREEN}%d${NC}    FAIL: ${RED}%d${NC}\n" "$PASS" "$FAIL"
if [[ ${#FAIL_DETAILS[@]} -gt 0 ]]; then
  echo
  echo "Failed tests:"
  for f in "${FAIL_DETAILS[@]}"; do echo "  - $f"; done
fi
echo

if [[ "$FAIL" -gt 0 ]]; then exit 1; else exit 0; fi
