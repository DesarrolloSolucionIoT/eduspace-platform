# scripts/

End-to-end endpoint test plan for `eduspace-platform` backend.

## Prerequisites

1. **Docker Desktop** with WSL integration enabled (the test runner needs MySQL on port 3308).
2. **dotnet-ef** CLI installed (`dotnet tool install --global dotnet-ef`).
3. **`.env` populated** at the repo root with:
   ```
   TokenSettings__Secret=<64-char hex — generate with: openssl rand -hex 32>
   TokenSettings__Issuer=EduSpace
   TokenSettings__Audience=EduSpaceUsers
   TokenSettings__AccessTokenLifetimeMinutes=60
   TokenSettings__RefreshTokenLifetimeDays=14
   CORS_ALLOWED_ORIGINS=http://localhost:5173,http://localhost:3000
   MYSQL_PORT=3308
   MYSQL_USER=eduspace
   MYSQL_PASSWORD=<strong>
   MYSQL_ROOT_PASSWORD=<strong>
   SENDGRID_API_KEY=<key or leave blank to use MockEmailService>
   ```
4. `jq` and `curl` installed.

## Bring everything up

```bash
cd /home/andres/code/iot/eduspace-platform
docker compose down -v
docker compose up -d mysql
cd FULLSTACKFURY.EduSpace.API
DOTNET_ROOT=/home/andres/.dotnet dotnet ef database update
dotnet run
```

## Run the tests

In a second terminal:

```bash
cd /home/andres/code/iot/eduspace-platform
./scripts/test-api.sh
```

By default it hits `http://localhost:5204`. Override:

```bash
API_BASE=http://localhost:7238 ./scripts/test-api.sh
```

## Coverage matrix

| # | Group | What it tests | Status without manual setup |
|---|-------|---------------|----------------------------|
| 0 | Sanity | Swagger UI reachable | runs |
| 1 | Auth happy path | sign-up, sign-in, verify-code | partial — verify-code needs `VERIFY_CODE=...` env (read from email or DB) |
| 2 | Authorization gate | GET protected endpoints without/garbage token → 401 | runs |
| 3 | Refresh rotation | refresh succeeds, used refresh = 401, logout, refresh after logout = 401 | needs valid ACCESS+REFRESH from group 1 |
| 4 | Domain validation | bad DNI / bad phone → 400 | needs ACCESS |
| 5 | Meeting conflict | overlapping reservations → 409 | manual fixture setup required |
| 6 | Report state machine | EnEspera → EnProceso → Completado transitions | manual fixture setup required |
| 7 | CORS | allowed/disallowed origin handling | runs |

## Getting a valid VERIFY_CODE without a real inbox

If you're using `MockEmailService` (no `SENDGRID_API_KEY`), the verification code is printed to the API console. Look for a line like `[MockEmail] code for testuser_XXXXXX is 123456` and re-run with:

```bash
VERIFY_CODE=123456 ./scripts/test-api.sh
```

If you're using real SendGrid, check the inbox of the email used in sign-up.

Alternatively, query MySQL directly:

```sql
SELECT vc.code
FROM verification_codes vc
JOIN accounts a ON a.id = vc.account_id
WHERE a.username = 'testuser_XXXXXX'
ORDER BY vc.id DESC LIMIT 1;
```

## Expected first-run output

```
==> 0. Sanity
PASS swagger reachable                                  [200]

==> 1. Auth happy path
PASS sign-up new user                                   [200]
PASS sign-in returns 200 (code sent)                    [200]
SKIP verify-code (requires email inbox or DB query — populate VERIFY_CODE env to run)

==> 2. Authorization gate
PASS GET /teachers-profiles without token → 401         [401]
PASS GET /classrooms without token → 401                [401]
PASS GET /meetings without token → 401                  [401]
PASS GET /reports without token → 401                   [401]
PASS GET /classrooms with garbage token → 401           [401]

==> 3. SKIPPED (VERIFY_CODE not set — cannot complete auth flow)

==> 4. (skipped — no access token)
==> 5. (skipped — fixtures not auto-seeded)
==> 6. (skipped — fixtures not auto-seeded)

==> 7. CORS preflight
PASS OPTIONS from allowed origin (vite localhost) → 204 [204]
PASS OPTIONS from disallowed origin                     no Allow-Origin for disallowed origin

============================================================
                          SUMMARY
============================================================
PASS: 9    FAIL: 0
```

## Notes

- **Group 1 sign-up payload** assumes `{username,email,password,role}`. Adjust if your `SignUpResource` differs.
- **Group 3 single-use refresh** assumes rotate-on-every-refresh semantics. If the design changes to sliding-window, update the assertion.
- **Group 4 DTO validation** assumes `[ApiController]` returns 400 on `ModelState.IsValid == false` automatically. If you removed that auto-behavior, the assertions need adjustment.
- **Groups 5 and 6** are manual on purpose — automating them requires fixture seeding (classrooms, admins, resources). Add seed data via EF `ModelBuilder.Entity<...>().HasData(...)` if you want them in CI.
