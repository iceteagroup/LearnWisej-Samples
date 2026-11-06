# AccessOps · Security & Auth Patterns · Module 5

Local lab build for **Module 5 · Protecting Data, Uploads, Downloads and Handlers**. The
confidential documents screen from the walkthrough - *AccessOps · Confidential documents*, the
toolbar (*Upload document*, *Download*, *Share link (1 h)*, `store: the AccessOpsData folder — outside
the web root`), the grid (*Document · Department · Class · Size · Updated*), the notice strip and
`… documents · streamed, never linked` - with an upload gate, downloads that only go through the
service, and a signed one-hour share link served by an ASP.NET Core endpoint.

> **Wisej.NET version.** These samples target the released **Wisej-4 4.1.7**. The Wisej.NET 4.2
> `Passkey` and `OAuthClient` components the course teaches in Modules 2 and 4 are not available;
> passkeys are not implemented and external sign-in uses ASP.NET Core's OpenID Connect handler (see
> Module 4). Nothing in this module's own lab depends on 4.2.

## Run it - two servers

```bash
cd "Security and Auth Patterns Course/Module 5/AccessOps.TestIdentityProvider"
dotnet run --urls http://localhost:6415          # TEST DOUBLE - never deploy

cd "Security and Auth Patterns Course/Module 5/AccessOps"
dotnet run -f net10.0 --urls http://localhost:6405
```

```bash
cd "Security and Auth Patterns Course/Module 5"
dotnet test          # 30 tests
```

Sign in with the work account as `ruth.okoye@…` (Finance, may upload) or `jordan.marsh@…`
(Facilities, no documents); demo passwords in
[users.json](AccessOps.TestIdentityProvider/Data/users.json). The first run generates small, valid
demo files for the seeded documents into `Module 5/AccessOpsData/documents`.

The share-link signing key comes from configuration supplied by the environment
(`ShareLinks__SigningKey`, base64, at least 32 bytes). Without one the app starts with a **DEMO
key** and logs a warning saying so - that is what the lab run used.

## What to try

| Action | Expected result |
|---|---|
| Ruth Okoye opens Documents | Four Finance documents, `4 documents · streamed, never linked` |
| Upload an executable renamed `payslip.pdf` | *payslip.pdf was refused: the file contents are not a PDF document. Upload a PDF, Word or Excel document up to 20 MB.*; status *Upload refused — signature check*; log `Signature (first bytes 4D 5A 90 00 — expected 25 50 44 46)`; nothing left in quarantine |
| Upload a real PDF `contract-4482.pdf` | *… accepted, scanned and filed under Finance. It is stored under a name the server generated.*; a toast *Filed: contract-4482.pdf* |
| Select *Supplier contract 4471.pdf*, **Download** | The file is streamed to this session with `Application.Download`; log `Download 4471 by employee 2101: allowed` |
| **Share link (1 h)** | A dialog with `http://localhost:6405/share/4471?exp=…&sig=…` |
| Open the link / edit `exp` / change the id to 9004 / drop `sig` / an old `exp` | 200 `application/pdf` / 404 / 404 / 404 / 404 - the same answer for every failure, each logged with the address |
| Jordan Marsh opens Documents | *No documents you may open* |

## Lab tasks → where in the code

| Deliverable | Where |
|---|---|
| Store outside the web root, server-generated names, original name as display text only | [DocumentStore.cs](AccessOps/Services/DocumentStore.cs), `Storage` in [appsettings.json](AccessOps/appsettings.json); Startup refuses to start if the store is under the web root |
| `Upload.MaxFileSize` and the host request limit at the same ceiling | [DocumentsView.cs](AccessOps/Pages/DocumentsView.cs) (`uploadDocument.MaxFileSize`), `ConfigureKestrel` in [Startup.cs](AccessOps/Startup.cs) |
| Upload gate: size, extension allow-list, content signature, scan, quarantine, link to the department | [UploadGate.cs](AccessOps/Services/UploadGate.cs), `DocumentService.Upload` |
| Downloads only through `DocumentService` (parameterised load, permission check, audit, `Application.Download`) | `DocumentService.Download`, [WisejFileDelivery.cs](AccessOps/Infrastructure/WisejFileDelivery.cs) |
| Signed, expiring share-link endpoint, mapped before `app.UseWisej()` | [ShareLinkEndpoint.cs](AccessOps/Infrastructure/ShareLinkEndpoint.cs), [ShareLinkService.cs](AccessOps/Services/ShareLinkService.cs) |
| Parameterised query and output encoding review | [docs/InjectionAndOutputReview.md](docs/InjectionAndOutputReview.md) |
| Test with a forged download request | [AccessOps.Tests/DocumentSecurityTests.cs](AccessOps.Tests/DocumentSecurityTests.cs) |

## Verified

- `dotnet test`: 30/30 (Modules 3-5 tests together), including the forged download, the five
  share-link tamper cases, the renamed executable, the size/extension/scan gates and the hostile
  search term.
- Browser pane (`accessops-m5` + temporary `accessops-idp-m5`): every row of *What to try*. Uploads
  were driven through the real Upload control by assigning a `File` to its `<input type=file>` with a
  `DataTransfer` from `javascript_tool`; the share-link probes were `fetch` calls from the page. A
  file named `<img src=x onerror=…>.pdf` was shown as text everywhere (see the review).

## Answers to the review questions

1. **The extension passed.** An attacker can still send any bytes with an allowed name - an
   executable called `payslip.pdf`, an HTML page called `report.pdf` that a browser might render.
   The signature check reads the content: `4D 5A` is not `%PDF-`, and an Office file must be a ZIP
   package with the `word/` or `xl/` parts.
2. **A forwarded share link.** The damage is bounded by the one-hour expiry, by the link naming one
   document only, and by the endpoint's log of every open with its address (and the rate limit in
   Module 7). Revoking a single link without changing the key needs server state: a link id inside
   the signed payload and a revocation list (or a per-document "links valid after" timestamp) checked
   by the endpoint.
3. **The hidden button and the forged request.** The test passes because of the line in
   `DocumentService.Authorized` that compares the document's department with the caller's -
   `CanRead(me, doc)` - and throws before anything is read. The button never existed for that
   caller; the request went straight to the service, which is the only thing that decides.

## Where this sample differs from the lesson

- **No naive starting version.** The lab's starting point describes an upload into the web root and
  a download by file name; the samples never ship that insecure version - Module 5 is the secured
  feature.
- **Scan.** "Run the scan" is a stand-in scanner that refuses the EICAR test string; a real
  deployment calls the organisation's malware scanner at that point.
- **Sizes.** The video shows sizes such as 412 KB and 1.2 MB; the seeded demo files are small
  generated documents (about 1 KB), and the grid shows their real size.
- **Board pack.** The intro lists the Executive board pack beside Finance documents; the Module 3
  department rule keeps it out of a Finance employee's list.
- **Status wording.** *Upload refused — audited* and *Download refused — audited* refer to the server
  log in this module; the audit table arrives in Module 6.
- **Passkeys / OAuthClient (Wisej.NET 4.2).** Not available on 4.1.7; see Modules 2 and 4.
