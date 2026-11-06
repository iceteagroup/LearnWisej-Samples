# Distribution plan and store listing draft

FieldOps technicians are employees on managed devices, so FieldOps is distributed **privately to one
organisation per platform**, not listed publicly.

## Enterprise distribution

| Platform | Channel | Setup | Who approves a new version | Rollout |
|---|---|---|---|---|
| Android | **Managed Google Play** (private app for the customer's organisation) | Upload the signed `.aab` in Play Console → *Advanced settings → Managed Google Play* → add the customer's organisation ID; customer assigns it in the EMM | Customer IT (mobility team) | Staged: pilot group *FieldOps-Pilot* (10 technicians), then *FieldOps-All* after 48 h with no crash spike |
| iOS (and macOS) | **Apple Business Manager custom app** | App Store Connect → *Pricing and Availability → Private distribution* to the customer's ABM organisation; the customer's MDM (Intune) pushes it to iPhones/Macs | Customer IT | MDM assignment groups, same pilot → all |
| Windows | **MSIX assigned through Intune** (line-of-business app) | Upload the signed MSIX to Intune; assignment *Required* for group *FieldOps-Depot-Laptops* | Customer IT desktop team | Pilot device group first |
| Windows (alternative) | Signed MSIX for direct download | Only for contractors outside Intune; same certificate | Contoso support | — |
| Browser / PWA | none — the server *is* the release | — | — | the PWA update flow (Module 2) |

On iOS, direct distribution outside ABM/App Store is essentially unavailable; enterprise in-house
programmes are not used.

## Store listing draft (also used for the private listings)

**Name:** FieldOps — Field Service Work Orders
**Short description:** Your assigned work orders, photos, signatures and navigation — on the job site.
**Description (for a technician):**
FieldOps shows the work orders your dispatcher assigned to you today. Open an order, mark it Accepted,
On site and Completed, take photos of the equipment, collect the customer's signature on the screen and
get directions to the next site. If the signal drops in a plant room, FieldOps keeps today's orders on
your device and sends your changes when you are back online. FieldOps is for employees and contractors
of organisations that use the FieldOps service; you sign in with your company account.

**Screenshots:** phone 1080 × 1920 (list, detail, signature), 7" and 10" tablet, Windows 1366 × 768,
Mac 1440 × 900. Taken from the Release build against the demo tenant; no real customer data.

**Support contact:** fieldops-support@contoso.example (demo address) · https://fieldops.example.com/help

## Privacy and data-safety declaration (honest list)

| Data | Collected | Why | Shared | Stored where | Optional |
|---|---|---|---|---|---|
| Photos (camera / photo library) | Yes, when the technician takes or attaches one | Evidence of the work on a work order | With the technician's organisation | FieldOps server, outside the web root | Yes (per photo) |
| Signature image + signer's typed name | Yes, when the customer signs | Proof of completion | With the organisation | FieldOps server | Yes |
| Precise location | Only when the technician presses Navigate | Distance to the site and arrival time | With the organisation | FieldOps server (lat, lon, accuracy, server time) | Yes — a refusal keeps the manual route |
| Device facts (platform, model, OS version, client version) | Yes | Compatibility check, support, usage telemetry | No | FieldOps server logs | No |
| Usage events (feature, outcome) | Yes | Which features work on which clients | No | FieldOps telemetry | No |
| Crash reports (exception type, message, build) | Yes | Fixing crashes | No (only the team's crash service) | crash service / `/client-log` | No |
| Contacts, microphone, calendar | **No** — the template's permissions for them are not used by FieldOps and should be removed before submission | — | — | — | — |

Telemetry never contains photo or signature contents, customer or signer names, or raw coordinates.
Data is encrypted in transit (HTTPS); deletion on request through the organisation's administrator.
