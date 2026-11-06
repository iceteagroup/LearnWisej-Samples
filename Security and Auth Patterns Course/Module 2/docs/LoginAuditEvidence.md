# Login audit records - evidence from a real run

The `login_audit` table after the Module 2 browser check (read with Python's `sqlite3` from
`Module 2/AccessOpsData/accessops.db`). Every attempt and every sign-out has a row; the screen
showed one sentence - *That user name and password combination is not valid.* - for rows 1-7.

| id | at (UTC) | user name tried | address | outcome |
|---|---|---|---|---|
| 1 | 19:57:08 | r.almeida | ::1 | BadCredential |
| 2 | 19:57:10 | r.almeida | ::1 | BadCredential |
| 3 | 19:57:31 | r.almeda | ::1 | UnknownUser |
| 4 | 19:57:33 | r.almeida | ::1 | BadCredential |
| 5 | 19:57:35 | r.almeida | ::1 | BadCredential |
| 6 | 19:57:37 | r.almeida | ::1 | BadCredential |
| 7 | 19:57:42 | r.almeida | ::1 | LockedOut - the **correct** password, refused without hashing |
| 12 | 20:01:17 | r.almeida | ::1 | Success (with *Remember this device*) |
| 13 | 20:01:31 | r.almeida | ::1 | RememberedDevice (new session, same browser) |
| 14 | 20:01:41 | r.almeida | ::1 | SignOutExplicit (device token revoked: `remember_tokens` count 0) |
| 15 | 20:03:04 | r.almeida | 127.0.0.1 | Success |
| 16 | 20:04:05 | r.almeida | 127.0.0.1 | SignOutIdle (test run with `sessionTimeout` 60 - 61 s later) |
| 21 | 20:06:12 | k.osei | 127.0.0.1 | Success |
| 22 | 20:07:13 | k.osei | 127.0.0.1 | SignOutIdle |

Rows 8-11 and 17-20 are further successful sign-ins and explicit sign-outs from the same session
of checks.

## What is stored instead of a password

```
('r.almeida', 'PBKDF2-SHA256', 210000, 16, 32)   -- algorithm, iterations, salt bytes, hash bytes
('k.osei',    'PBKDF2-SHA256', 210000, 16, 32)
```

The `employees` table has `pw_algorithm`, `pw_iterations`, `pw_salt`, `pw_hash` - and no password
column. The demo passwords in `Data/seed-credentials.json` exist only so the lab can be signed in
to; they are hashed through `PasswordService.Hash` when the database is created.

## Reading the table yourself

```bash
python -c "import sqlite3; c=sqlite3.connect('Module 2/AccessOpsData/accessops.db'); print(*c.execute('select * from login_audit'), sep='\n')"
```
