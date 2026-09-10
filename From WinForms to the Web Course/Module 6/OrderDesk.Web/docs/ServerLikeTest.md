# Testing on a clean server-like environment (lab deliverable 5)

Lab step covered: *test on a clean server-like environment*. A migrated file/report feature is not finished when it
works on the developer's PC — that PC has Office, a printer, `C:\Orders`, and an administrator account. The server has
none of them. Three environments, in increasing strictness.

## 1. Same PC, "no Office / no C:\Orders" pass

1. Make sure `C:\Orders` does **not** exist (or rename it). *Legacy import* must fail with
   `DirectoryNotFoundException` and the red banner; *Upload* must still work.
2. *Legacy Excel Interop*: if Office is installed the banner is amber (KB 257757 explanation); the check is that
   **no `EXCEL.EXE` appears in Task Manager** — the console never creates the COM object. If you can, test on a machine
   without Office: the banner turns red ("ProgID missing").
3. *Export .xlsx*: open the downloaded `Orders.xlsx` in Excel or LibreOffice — one sheet "Orders", numeric Total.
4. Delete `App_Data/` entirely and start the app again: the root and the three sub-folders are created on demand
   (`Load` trace shows `0 file(s)`).

## 2. Non-admin service account (Windows)

The web app runs as a service account (IIS AppPool identity, a `LocalService`-style account, or a plain standard user).

1. Create a standard (non-admin) local user, or use `runas /user:<standard-user>` for the `dotnet run` command.
2. Grant that user **Modify** on the project folder only (it needs to create `App_Data/…`). Grant nothing else.
3. Run `dotnet run -f net10.0 --urls http://localhost:5606` as that user and repeat the card B/C/D buttons:
   * Upload → `App_Data\uploads\order-batch.csv` is created **under the project**, nowhere else.
   * *Legacy import* must fail (`DirectoryNotFoundException`, or `UnauthorizedAccessException` if someone created
     `C:\Orders` with tight ACLs) — both are caught and explained.
   * Queue jobs → files under `App_Data\reports\` owned by the service account. Nothing is written to the user profile,
     `%APPDATA%`, `HKCU`, or a printer spooler.
4. Point `OrderDesk.StorageRoot` in `Web.config` to an **absolute** path the account can write (`D:\OrderDeskData`)
   and restart: the trace shows the new root; the code path is identical (`Path.IsPathRooted` decides).

## 3. Linux container (the strictest)

Nothing Windows-specific may remain on the migrated path: no `\`, no drive letters, no registry, no COM, no
`System.Drawing.Printing`, no assumption about case. The project multi-targets `net10.0` for exactly this.

```bash
# from "Module 6/OrderDesk.Web" on a Linux box or in WSL
dotnet build -nologo -v q -f net10.0            # 0 warnings, 0 errors (the Windows-only surface is either excluded or guarded)
dotnet run -f net10.0 --urls http://0.0.0.0:5606
```

Or in a container (no Dockerfile ships with the sample; this is the whole recipe):

```bash
docker run --rm -it -p 5606:5606 -v "$PWD":/app -w /app mcr.microsoft.com/dotnet/sdk:10.0 \
  dotnet run -f net10.0 --urls http://0.0.0.0:5606
```

What to check there:

| Button | Expected on Linux |
|---|---|
| Load trace | `storage root  C:\Orders\…  ⇒  /app/App_Data  (OrderDesk.StorageRoot = "App_Data" · 0 file(s) · separator '/')` and `OS Linux …` |
| Upload | `/app/App_Data/uploads/order-batch.csv`; five orders saved exactly as on Windows |
| Legacy import | **`FileNotFoundException`**, not `DirectoryNotFoundException`: on Linux `C:\Orders\in.csv` is a relative file name with backslashes in it. The banner text is the same; the trace shows the exception type |
| Legacy Excel Interop | red banner "✕ Linux … : no COM, no registry, no Excel.Application — the desktop export cannot even ask for the ProgID" (`OperatingSystem.IsWindows()` is false; `Type.GetTypeFromProgID` is never called) |
| Export .xlsx / Print → PDF / queue | identical bytes to Windows: `ZipArchive`, `InvoicePdfWriter` and `File.WriteAllBytes(Path.Combine(...))` are platform-neutral |
| Case sensitivity | `ls App_Data` shows `exports  reports  uploads` — one spelling, created by `StorageRoot.Ensure`; nothing else composes those names |

### Linux path notes (what to grep for before deploying)

* `grep -rn '\\\\' --include=*.cs` on the migrated path should find only string *content* (banner texts, the legacy
  constants in `Legacy/`), never a path being built.
* `Path.Combine`, `Path.GetFileName`, `Path.GetRelativePath`, `Path.DirectorySeparatorChar` everywhere a path is made
  or shown (`StorageRoot`, `ReportQueue.Run`, `ReportService`, `MainPage.Relative`).
* `Application.StartupPath` is `/app` in the container, the project folder under `dotnet run` on a workstation, the
  site folder under IIS — never hard-code any of them.
* Folder names are lower-case and used from one place only (`StorageRoot.Uploads/Exports/Reports`).

## Evidence

The console cannot prove the environment it runs in, so it prints it: the Load trace line
`• server StorageRoot  uploads/ exports/ reports/ under App_Data · OS Microsoft Windows 10.0.26200` (or `OS Linux …`).
Compare that line, the separator shown in the storage-root line, and the exception type in the *Legacy import* banner
between your workstation run and the server-like run — they are the three things that differ, and the rest of the
console must behave identically.

Builds verified on this machine (Windows 11, .NET SDK 10.0.303): `dotnet build -nologo -v q -f net10.0` and
`-f net10.0-windows` both succeed with 0 warnings, 0 errors. The Linux run itself was not executed here — see the
README's *Runtime facts*.
